using ApplicationCore.Consts;
using ApplicationCore.Settings;
using System.Text;
using System.Text.Json;
using Serilog;
using ApplicationCore.Views;
using Microsoft.Extensions.Caching.Memory;
using ApplicationCore.Exceptions;

namespace ApplicationCore.Services;
public interface ISSOService
{
   string ResolveUserSSOToken(string jwt);
   Task<SSOAuthResponse?> GetAppTokenAsync();
   Task<bool> ValidateUserTokenAsync(string token);
   Task<SSOUserAuthInfo?> GetUserAuthInfoAsync(string token);
   Task<SSOUserUUIDInfo?> GetUserUUIDInfoAsync(string token);
   Task<SSOUserProfilesResponse> GetUserProfilesInfoAsync(string token, SSOUserUUIDInfo uuidInfo);
}

public class SSOService : ISSOService
{
   private readonly IHttpClientFactory _httpClientFactory;
   private readonly SSOSettings _ssoSettings;
   private readonly IMemoryCache _cache;
   private readonly ILogger _logger;
   public SSOService(IHttpClientFactory httpClientFactory, SSOSettings ssoSettings, IMemoryCache cache)
   {
      _httpClientFactory = httpClientFactory;
      _ssoSettings = ssoSettings;
      _cache = cache;
      _logger = Log.ForContext<SSOService>(); // 建立具類別名稱的 logger context
      _cache = cache;
   }

   private const string TOKEN_CACHE_KEY = "SSO_TOKEN";
   void RemoveTokenCache() => _cache.Remove(TOKEN_CACHE_KEY);
   public string ResolveUserSSOToken(string jwt)
   {
      var parts = jwt.Split('.');
      var payload = parts[1];
      payload = payload.Replace('-', '+').Replace('_', '/');
      switch (payload.Length % 4)
      {
         case 2: payload += "=="; break;
         case 3: payload += "="; break;
      }
      var jsonBytes = Convert.FromBase64String(payload);
      var json = Encoding.UTF8.GetString(jsonBytes);
      var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

      if (dict!.TryGetValue("PUBLIC_APP_USER_SSO_TOKEN", out var value)) return value.ToString()!;
      return "";
   }

   public async Task<SSOAuthResponse?> GetAppTokenAsync()
   {
      if (_cache.TryGetValue(TOKEN_CACHE_KEY, out SSOAuthResponse cachedToken))
      {
         _logger.Debug("Using cached SSO token.");
         return cachedToken;
      }

      _logger.Information("Requesting new SSO token...");
      var payload = new
      {
         APP_PRIVATE_ID = _ssoSettings.PrivateId,
         APP_PRIVATE_PASSWD = _ssoSettings.Password
      };
      string action = "/app/request_basic_authentication/";
      var response = await ForwardPostAsync(action, payload);
      if (string.IsNullOrEmpty(response))
      {
         OnSSOError(action, "");
         return null;
      }
      else
      {
         var authResponse = JsonSerializer.Deserialize<SSOAuthResponse>(response);
         if (authResponse!.ERROR_CODE == "0")
         {
            _logger.Information("SSO token: {Token}", authResponse.PRIVATE_APP_SSO_TOKEN);
            DateTime expiry = ParseSSODate(authResponse.PRIVATE_APP_SSO_TOKEN_EXPIRY_DATE);
            _cache.Set(TOKEN_CACHE_KEY, authResponse, expiry - DateTime.Now);
            _logger.Information("SSO token cached until {Expiry}", expiry);

            return authResponse;
         }
         else
         {
            OnSSOError(action, response);
            return null;
         }
      }
   }
   public async Task<bool> ValidateUserTokenAsync(string token)
   {
      //WS-Z01-B0-04  驗證某筆登入用戶權杖的公開有效性
      var appTokens = await GetAppTokenAsync();

      _logger.Information("Validate UserToken ...");
      var payload = new
      {
         PRIVILEGED_APP_SSO_TOKEN = appTokens!.PRIVILEGED_APP_SSO_TOKEN,
         PUBLIC_APP_USER_SSO_TOKEN_TO_VALIDATE = token
      };
  
      string action = "/app_user/validate_sso_token/";
      var response = await ForwardPostAsync(action, payload);
      if (string.IsNullOrEmpty(response))
      {
         OnSSOError(action, "");
         return false;
      }
      else
      {
         var model = JsonSerializer.Deserialize<BaseSSOResponse>(response);
         if (model!.ERROR_CODE == "0")
         {
            _logger.Information("success ValidateUserToken : {Token}", token);
            return true;
         }
         return false;
      }
   }
   public async Task<SSOUserAuthInfo?> GetUserAuthInfoAsync(string token)
   {
      //WS-Z01-B0-05  查詢登入用戶認證資訊
      var appTokens = await GetAppTokenAsync();

      _logger.Information("Get User Auth Info ...");
      var payload = new
      {
         PRIVILEGED_APP_SSO_TOKEN = appTokens!.PRIVILEGED_APP_SSO_TOKEN,
         PUBLIC_APP_USER_SSO_TOKEN_TO_QUERY = token
      };

      string action = "/app_user/get_auth_info/";
      var response = await ForwardPostAsync(action, payload);
      if (string.IsNullOrEmpty(response))
      {
         OnSSOError(action, "");
         return null;
      }
      else
      {
         var model = JsonSerializer.Deserialize<SSOUserAuthInfo>(response);
         if (model!.ERROR_CODE == "0") return model;
         else
         {
            OnSSOError(action, response);
            return null;
         }
      }
   }
   public async Task<SSOUserUUIDInfo?> GetUserUUIDInfoAsync(string token)
   {
      //WS-Z01-B0-06  查詢登入用戶內部代碼 
      var appTokens = await GetAppTokenAsync();

      _logger.Information("Get User UUID Info ...");
      var payload = new
      {
         PRIVILEGED_APP_SSO_TOKEN = appTokens!.PRIVILEGED_APP_SSO_TOKEN,
         PUBLIC_APP_USER_SSO_TOKEN_TO_QUERY = token
      };
      string action = "/app_user/get_node_uuid/";
      var response = await ForwardPostAsync(action, payload);
      if (string.IsNullOrEmpty(response))
      {
         OnSSOError(action, "");
         return null;
      }
      else
      {
         var model = JsonSerializer.Deserialize<SSOUserUUIDInfo>(response);
         if (model!.ERROR_CODE == "0") return model;
         else
         {
            OnSSOError(action, response);
            return null;
         }
      }
   }
   public async Task<SSOUserProfilesResponse> GetUserProfilesInfoAsync(string token, SSOUserUUIDInfo uuidInfo)
   {
      //WS-Z01A-S-B05：取回登入用戶單一人員屬性 
      var appTokens = await GetAppTokenAsync();
      var payload = new
      {
         PRIVILEGED_APP_SSO_TOKEN = appTokens!.PRIVILEGED_APP_SSO_TOKEN,
         PUBLIC_APP_USER_SSO_TOKEN = token,
         APP_COMPANY_UUID = uuidInfo.APP_COMPANY_UUID,
         APP_USER_NODE_UUID = uuidInfo.APP_USER_NODE_UUID,
         APP_USER_BASIC_PROFILE = new {
            APP_USER_LOGIN_ID = "",
            APP_USER_CHT_NAME = "",
            APP_USER_OFFICE_PHONE_NO = "",
            APP_USER_EMAIL = "",
            APP_DEPT_NODE_UUID = "",
            APP_USER_STATUS = "",
         },
         APP_USER_EMPLOY_PROFILE = new
         {
            APP_USER_EMPLOY_TITLE = ""
         },
         APP_USER_EIP_PROFILE = new
         {
            AD_DEPARTMENT = "",
            AD_COMPANY = ""
         },
         APP_USER_NODE_LAST_UPDATE_TIME = "",
         APP_USER_NODE_LAST_UPDATE_TAG = ""
      };
      string action = "/org_tree_surrogate/get_user_node/";
      var response = await ForwardPostAsync(action, payload);
      if (string.IsNullOrEmpty(response))
      {
         OnSSOError(action, "");
         return null;
      }
      else
      {
         var model = JsonSerializer.Deserialize<SSOUserProfilesResponse>(response);
         if (model!.ERROR_CODE == 0)
         {
            return model;
         }
         else
         {
            OnSSOError(action, response);
            return null;
         }
      }
   }
   private async Task<string?> ForwardPostAsync(string url, object payload)
   {
      var client = _httpClientFactory.CreateClient(SettingsKeys.SSO);
      var json = JsonSerializer.Serialize(payload);
      var content = new StringContent(json, Encoding.UTF8, "application/json");

      try
      {
         var response = await client.PostAsync(url, content);
         response.EnsureSuccessStatusCode();
         
         var result = await response.Content.ReadAsStringAsync();
         _logger.Information("SSO request succeeded: {Url}", url);
         return result;
      }
      catch (HttpRequestException ex)
      {
         _logger.Error(ex, "HTTP error calling {Url}", url);
         return null;
      }
      catch (Exception ex)
      {
         _logger.Fatal(ex, "Unexpected error calling {Url}", url);
         return null;
      }
   }

   private static DateTime ParseSSODate(string raw)
   {
      // "20251108144729+0800"
      if (DateTime.TryParseExact(raw[..14], "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out var dt))
         return dt;
      return DateTime.UtcNow.AddHours(1);
   }

   private void OnSSOError(string action, string response)
   {
      var model = JsonSerializer.Deserialize<BaseSSOResponse>(response);
      if (!string.IsNullOrEmpty(model.ERROR_CODE))
      {
         if (model.ERROR_CODE == "451") RemoveTokenCache();
      }
      _logger.Error(new SSOException($"action: {action} , response: {response}"), "Unexpected error calling {action}", action);
   }
}
