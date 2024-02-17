using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Identity;

namespace ApplicationCore.DataAccess;
public class DefaultContext : IdentityDbContext<User, Role, string,
        IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
{
  
   public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
	{
	}
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
   }
   //protected override void OnModelCreating(ModelBuilder builder)
   //{
   //	base.OnModelCreating(builder);
   //	builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
   //	var types = builder.Model.GetEntityTypes()
   //					.SelectMany(t => t.GetProperties())
   //					.Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));
   //	foreach (var property in types)
   //	{
   //		property.SetColumnType("timestamp without time zone");
   //	}
   //}
  
   public DbSet<Profiles> Profiles => Set<Profiles>();
   public DbSet<Department> Departments => Set<Department>();
   public DbSet<Job> Jobs => Set<Job>();


   #region Auth
   public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
	public DbSet<OAuth> OAuth => Set<OAuth>();
   #endregion


   #region Articles	
   public DbSet<Category> Categories => Set<Category>();
   public DbSet<Article> Articles => Set<Article>();
   public DbSet<UploadFile> UploadFiles => Set<UploadFile>();
	#endregion

	public override int SaveChanges() => SaveChangesAsync().GetAwaiter().GetResult();

}
