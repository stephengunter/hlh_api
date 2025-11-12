using System.Text.Json.Serialization;

namespace ApplicationCore.Views;

public class BaseSSOResponse
{
   public string ERROR_CODE { get; set; } = string.Empty;
   public string ERROR_MESSAGE { get; set; } = string.Empty;
}
public class SSOAuthResponse : BaseSSOResponse
{   
   public string PUBLIC_APP_SSO_TOKEN { get; set; } = string.Empty;
   public string PRIVILEGED_APP_SSO_TOKEN { get; set; } = string.Empty;
   public string PRIVATE_APP_SSO_TOKEN { get; set; } = string.Empty;
   public string PUBLIC_APP_SSO_TOKEN_EXPIRY_DATE { get; set; } = string.Empty;
   public string PRIVILEGED_APP_SSO_TOKEN_EXPIRY_DATE { get; set; } = string.Empty;
   public string PRIVATE_APP_SSO_TOKEN_EXPIRY_DATE { get; set; } = string.Empty;
}

public class SSOUserAuthInfo : BaseSSOResponse
{
   public string APP_COMPANY_ID { get; set; } = string.Empty;  //"judicial"
   public string APP_USER_LOGIN_ID { get; set; } = string.Empty;  //"stephen.chung"
}
public class SSOUserUUIDInfo : BaseSSOResponse
{
   public string APP_COMPANY_UUID { get; set; } = string.Empty;
   public string APP_USER_NODE_UUID { get; set; } = string.Empty;
}

public class SSOUserProfilesResponse : BaseSSOResponse
{
   public int ERROR_CODE { get; set; }
   public string ERROR_MESSAGE { get; set; } = string.Empty;
   public string APP_USER_NODE_UUID { get; set; } = string.Empty;
   public SSOUserBasicProfiles APP_USER_BASIC_PROFILE { get; set; } = new SSOUserBasicProfiles();
   public SSOUserEmployProfiles APP_USER_EMPLOY_PROFILE { get; set; } = new SSOUserEmployProfiles();
   public SSOUserEIPProfiles APP_USER_EIP_PROFILE { get; set; } = new SSOUserEIPProfiles();
   public string APP_USER_NODE_LAST_UPDATE_TIME { get; set; } = string.Empty;
   public string APP_USER_NODE_LAST_UPDATE_TAG { get; set; } = string.Empty;
}

public class SSOUserBasicProfiles
{
   public string APP_USER_LOGIN_ID { get; set; } = string.Empty;
   public string APP_USER_CHT_NAME { get; set; } = string.Empty;
   public string APP_USER_OFFICE_PHONE_NO { get; set; } = string.Empty;
   public string APP_USER_EMAIL { get; set; } = string.Empty;
   public string APP_DEPT_NODE_UUID { get; set; } = string.Empty;
   public int APP_USER_STATUS { get; set; }
}
public class SSOUserEmployProfiles
{
   public string APP_USER_EMPLOY_TITLE { get; set; } = string.Empty;
}
public class SSOUserEIPProfiles
{
   public string AD_DEPARTMENT { get; set; } = string.Empty;
   public string AD_COMPANY { get; set; } = string.Empty;
}
