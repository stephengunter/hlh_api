using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Helpers;
using ApplicationCore.Consts;
using ApplicationCore.Models.Auth;
using ApplicationCore.Views.AD;
using ApplicationCore.Settings;
using Novell.Directory.Ldap;

namespace ApplicationCore.Services.Auth;

public interface ILdapService
{
   IEnumerable<AdUser> FetchAll();
}

public class LdapService : ILdapService
{
   private readonly LdapSettings _settings;

   private string DOMAIN = "ad.intraj";
   string BASE_ON = "OU=HLH,DC=ad,DC=intraj";
   public LdapService(LdapSettings settings)
   {
      _settings = settings;
   }

   string AdminLdapRdn => $"{_settings.AdminUser}@{DOMAIN}";

   public bool CheckAuth(string username, string password)
   {
      try
      {
         using (var ldapConnection = new LdapConnection())
         {
            string ldapRdn = $"{username}@{DOMAIN}";
            ldapConnection.Connect(_settings.Server, _settings.Port);
            ldapConnection.Bind(ldapRdn, password);
         }
         return true;
      }
      catch (LdapException ex)
      {
         Console.WriteLine($"LDAP query failed: {ex.Message}");
         return false;
      }
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
            var attributes = entry.GetAttributeSet();
            string username = GetAttributeValue(attributes, "cn");
            string title = GetAttributeValue(attributes, "displayName");
            string dpt = GetAttributeValue(attributes, "department");

            users.Add(new AdUser
            {
               Username = username,
               Title = title,
               Department = dpt
            });
         }
      }
      return users;
   }

   private string GetAttributeValue(LdapAttributeSet attributes, string attributeName)
   {
      var attribute = attributes.GetAttribute(attributeName);
      if (attribute != null) return attribute.StringValue;
      return "";
   }
   void AD()
   {
      string domain = "ad";
      string ldapHost = "172.17.128.54";         // Your LDAP server host
      //string baseDn = "DC=ad,DC=intraj";       // Your Base DN
      string baseDn = "OU=HLH,DC=ad,DC=intraj";
      string username = "stephen.chung";       // The username to search for
      string ldapRdn = $"{username}@{domain}.intraj";
      //string ldapRdn = "CN=admin,CN=Users,DC=ad,DC=intraj";  // Your LDAP bind DN (admin or service account)
      string ldapPassword = "Stephen@79";   // Password for the LDAP bind DN

      var ldapHelper = new LdapHelper();
      ldapHelper.SearchUser(ldapHost, baseDn, ldapRdn, ldapPassword, username);
   }

   public void SearchUser(string ldapHost, string baseDn, string ldapRdn, string ldapPassword, string username)
   {
      try
      {
         using (var ldapConnection = new LdapConnection())
         {
            ldapConnection.Connect(ldapHost, 389); // Connect to LDAP server
            ldapConnection.Bind(ldapRdn, ldapPassword); // Bind with LDAP RDN and password

            // Search for the user using the sAMAccountName or other attributes
            //string searchFilter = $"(sAMAccountName={username})";
            string searchFilter = "(&(objectClass=user))";
            var searchResults = ldapConnection.Search(
                baseDn, // Base DN of your AD
                LdapConnection.ScopeSub,
                searchFilter,
                null, // Null means all attributes will be returned
                false
            );

            while (searchResults.HasMore())
            {
               var entry = searchResults.Next();
               Console.WriteLine($"User DN: {entry.Dn}");
               Console.WriteLine("Attributes:");

               foreach (var attribute in entry.GetAttributeSet())
               {
                  Console.WriteLine($"{attribute.Name}: {attribute.StringValue}");
               }
            }
         }
      }
      catch (LdapException ex)
      {
         Console.WriteLine($"LDAP query failed: {ex.Message}");
      }
   }
}
