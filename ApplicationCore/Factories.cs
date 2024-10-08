using ApplicationCore.Consts;
using ApplicationCore.DataAccess;
using ApplicationCore.Services.Auth;
using ApplicationCore.Settings;
using Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ApplicationCore
{
   public static class Factories
   {
      public static DefaultContext CreateDefaultContext(string connectionString, DbSettings settings)
      {
         if (settings.Provider.EqualTo(DbProvider.PostgreSql)) 
         {
            return new DefaultContext(new DbContextOptionsBuilder<DefaultContext>().UseNpgsql(connectionString).Options);
         }
         return new DefaultContext(new DbContextOptionsBuilder<DefaultContext>().UseSqlServer(connectionString).Options);
      }
      public static ILdapService CreateLdapService(LdapSettings ldapSettings)
         => new LdapService(ldapSettings);
   }
}
