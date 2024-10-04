using Microsoft.AspNetCore.Mvc;
using ApplicationCore.DataAccess;
using Microsoft.SqlServer.Dac;
using Ardalis.Specification;
using System.Text.Json;
using System.IO;
using Novell.Directory.Ldap;

namespace Web.Controllers.Tests;

public class AATestsController : BaseTestController
{
   public AATestsController(DefaultContext defaultContext)
   {
   }

   

   [HttpGet]
   public async Task<ActionResult> Index()
   {
      AD();

      return Ok();
   }

   string Test()
   {
      string username = "stephen.chung";
      string password = "Stephen@79";
      var ldapServer = "172.17.128.54";
      var ldapPort = 389;

      try
      {
         using (var ldapConnection = new LdapConnection())
         {
            string domain = "ad"; // The domain name

            string ldapRdn = $"{username}@{domain}.intraj"; // Example: "john@ad.com"

            ldapConnection.Connect(ldapServer, ldapPort); // Connect to LDAP
            ldapConnection.Bind(ldapRdn, password); // Authenticate with the server

            // Query the Root DSE to get the default naming context (Base DN)
            var searchResults = ldapConnection.Search(
                "",
                LdapConnection.ScopeBase,
                "(objectClass=*)",
                new[] { "defaultNamingContext" },
                false
            );

            // Retrieve the defaultNamingContext (Base DN)
            if (searchResults.HasMore())
            {
               var entry = searchResults.Next();
               var baseDn = entry.GetAttribute("defaultNamingContext").StringValue;
               Console.WriteLine($"Base DN: {baseDn}");
               return baseDn;
            }
         }
      }
      catch (LdapException ex)
      {
         Console.WriteLine($"LDAP query failed: {ex.Message}");
         return "";
      }
      return "";
      //var baseDn = _config["Ldap:BaseDn"];
      //var adminDn = _config["Ldap:AdminDn"];
      //var adminPassword = _config["Ldap:AdminPassword"];

      //using var ldapConnection = new LdapConnection();
      //ldapConnection.Connect(ldapServer, ldapPort);

      //ldapConnection.Bind(ldapRdn, "Stephen@79");

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

   bool IsValid(string val)
   {
      if (string.IsNullOrEmpty(val)) return false;
      if (val.Length != 6) return false;
      if (val.FirstOrDefault().ToString().ToUpper() != "U") return false;
      return true;
   }
   void ExportDatabaseToBacpac(string connectionString, string bacpacFilePath)
   {
      try
      {
         // Create an instance of DacServices with the connection string
         DacServices dacServices = new DacServices(connectionString);

         // Subscribe to the Message event to receive status messages
         dacServices.Message += (sender, e) => Console.WriteLine(e.Message);

         Console.WriteLine("Starting export...");

         // Perform the export
         dacServices.ExportBacpac(bacpacFilePath, "hlh_api");

         Console.WriteLine($"Export completed. Bacpac file saved to: {bacpacFilePath}");
      }
      catch (Exception ex)
      {
         Console.WriteLine($"An error occurred: {ex.Message}");
      }
   }
}
public class LdapHelper
{
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