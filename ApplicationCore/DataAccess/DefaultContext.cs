using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;

namespace ApplicationCore.DataAccess;
public class DefaultContext : IdentityDbContext<User>
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
   public DbSet<Profile> Profiles => Set<Profile>();

   #region Auth
   public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
	public DbSet<OAuth> OAuth => Set<OAuth>();
	#endregion

	#region Articles	
	public DbSet<Article> Articles => Set<Article>();
   public DbSet<Department> Departments => Set<Department>();
   public DbSet<UploadFile> UploadFiles => Set<UploadFile>();
	public DbSet<Category> Categories => Set<Category>();
	#endregion

	public override int SaveChanges() => SaveChangesAsync().GetAwaiter().GetResult();

}
