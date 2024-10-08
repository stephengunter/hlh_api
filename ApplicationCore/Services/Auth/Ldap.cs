using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Helpers;
using ApplicationCore.Consts;
using ApplicationCore.Models.Auth;
using ApplicationCore.Views.AD;
using ApplicationCore.Settings;
using Novell.Directory.Ldap;
using Infrastructure.Helpers;

namespace ApplicationCore.Services.Auth;

public interface ILdapService
{
   IEnumerable<AdUser> FetchAll();
   AdUser? CheckAuth(string username, string password);
}

public class LdapService : ILdapService
{
   private readonly LdapSettings _settings;

   private string DOMAIN = "ad.intraj";
   private string BASE_ON = "OU=HLH,DC=ad,DC=intraj";
   private string ATTR_CN = "cn";
   private string ATTR_DISPLAY_NAME = "displayName";
   private string ATTR_DEPARTMENT = "department";
   public LdapService(LdapSettings settings)
   {
      _settings = settings;
   }

   string AdminLdapRdn => $"{_settings.AdminUser}@{DOMAIN}";
   List<string> AttrKeys => new List<string> { ATTR_CN, ATTR_DISPLAY_NAME, ATTR_DEPARTMENT };

   public AdUser? CheckAuth(string username, string password)
   {
      try
      {
         using (var ldapConnection = new LdapConnection())
         {
            string ldapRdn = $"{username}@{DOMAIN}";
            ldapConnection.Connect(_settings.Server, _settings.Port);
            ldapConnection.Bind(ldapRdn, password);

            string searchFilter = $"(sAMAccountName={username})";
            var searchResults = ldapConnection.Search(
                BASE_ON,
                LdapConnection.ScopeSub,
                searchFilter,
                AttrKeys.ToArray(),
                false
            );

            while (searchResults.HasMore())
            {
               var entry = searchResults.Next();
               return ResolveAdUser(entry);
            }
            return null;
         }
      }
      catch (LdapException ex)
      {
         Console.WriteLine($"LDAP query failed: {ex.Message}");
         return null;
      }
   }

   AdUser ResolveAdUser(LdapEntry entry)
   {
      var attributes = entry.GetAttributeSet();
      string userName = GetAttributeValue(attributes, ATTR_CN);
      string title = GetAttributeValue(attributes, ATTR_DISPLAY_NAME);
      string dpt = GetAttributeValue(attributes, ATTR_DEPARTMENT);

      return new AdUser
      {
         Username = userName,
         Title = title,
         Department = dpt
      };
   }
   public IEnumerable<AdUser> FetchAll()
   {
      var users = new List<AdUser>();
      using (var ldapConnection = new LdapConnection())
      {
         ldapConnection.Connect(_settings.Server, _settings.Port);
         ldapConnection.Bind(AdminLdapRdn, _settings.AdminPassword);
         
         string searchFilter = "(&(objectClass=user))";
         var searchResults = ldapConnection.Search(
             BASE_ON, 
             LdapConnection.ScopeSub,
             searchFilter,
             null,
             false
         );

         while (searchResults.HasMore())
         {
            var entry = searchResults.Next();
            users.Add(ResolveAdUser(entry));
         }
      }
      return users;
   }

   private string GetAttributeValue(LdapAttributeSet attributes, string attributeName)
   {
      if (attributes.Keys.Contains(attributeName))
      {
         var attribute = attributes.GetAttribute(attributeName);
         if (attribute != null) return attribute.StringValue;
      }
      
      return "";
   }
   //void AD()
   //{
   //   string domain = "ad";
   //   string ldapHost = "172.17.128.54";         // Your LDAP server host
   //   //string baseDn = "DC=ad,DC=intraj";       // Your Base DN
   //   string baseDn = "OU=HLH,DC=ad,DC=intraj";
   //   string username = "stephen.chung";       // The username to search for
   //   string ldapRdn = $"{username}@{domain}.intraj";
   //   //string ldapRdn = "CN=admin,CN=Users,DC=ad,DC=intraj";  // Your LDAP bind DN (admin or service account)
   //   string ldapPassword = "Stephen@79";   // Password for the LDAP bind DN

   //   var ldapHelper = new LdapHelper();
   //   ldapHelper.SearchUser(ldapHost, baseDn, ldapRdn, ldapPassword, username);
   //}
}
