using ApplicationCore.Models;
using ApplicationCore.Models.Auth;
using ApplicationCore.Models.Cars;
using ApplicationCore.Models.IT;
using ApplicationCore.Models.Keyin;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using ApplicationCore.Models.Criminals;
using ApplicationCore.Models.Fetches;

namespace ApplicationCore.DataAccess;
public class DefaultContext : IdentityDbContext<User, Role, string,
        IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
{
  
   public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
	{
      
   }
   protected override void OnModelCreating(ModelBuilder builder)
   {
      base.OnModelCreating(builder);
      builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

      if (Database.IsNpgsql())
      {
         var types = builder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?));
         foreach (var property in types)
         {
            property.SetColumnType("timestamp without time zone");
         }
      }
   }
   public DbSet<TypeCategory> TypeCategories => Set<TypeCategory>();
   public DbSet<Branch> Branches => Set<Branch>();
   public DbSet<TelName> TelNames => Set<TelName>();
   public DbSet<ModifyRecord> ModifyRecords => Set<ModifyRecord>();
   public DbSet<Profiles> Profiles => Set<Profiles>();
   public DbSet<Department> Departments => Set<Department>();
   public DbSet<Location> Locations => Set<Location>();
   public DbSet<JobTitle> JobTitles => Set<JobTitle>();
   public DbSet<Job> Jobs => Set<Job>();
   public DbSet<JobUserProfiles> JobUserProfiles => Set<JobUserProfiles>();


   #region Auth
   public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
	public DbSet<OAuth> OAuth => Set<OAuth>();
   public DbSet<AuthToken> AuthTokens => Set<AuthToken>();
   #endregion

   public DbSet<Court> Courts => Set<Court>();

   #region Cars
   public DbSet<Car> Cars => Set<Car>();
   public DbSet<CarType> CarTypes => Set<CarType>();
   public DbSet<CarRent> CarRents => Set<CarRent>();
   #endregion

   #region IT
   public DbSet<Host> Hosts => Set<Host>();
   public DbSet<SystemApp> SystemApps => Set<SystemApp>();
   public DbSet<CredentialInfo> CredentialInfoes => Set<CredentialInfo>();
   public DbSet<Database> Databases => Set<Database>();
   public DbSet<DbBackupPlan> DbBackupPlans => Set<DbBackupPlan>();
   public DbSet<DbBackupTask> DbBackupTasks => Set<DbBackupTask>();
   public DbSet<SystemAppDatabase> SystemAppDatabases => Set<SystemAppDatabase>();
   #endregion

   #region Keyin
   public DbSet<KeyinPerson> KeyinPersons => Set<KeyinPerson>();
   public DbSet<BranchRecord> KeyinBranchRecords => Set<BranchRecord>();
   public DbSet<PersonRecord> KeyinPersonRecord => Set<PersonRecord>();
   #endregion

   #region Criminal
   public DbSet<CriminalFetchRecord> CriminalFetchRecords => Set<CriminalFetchRecord>();
   #endregion
   #region Fetches
   public DbSet<FetchesRecord> FetchesRecords => Set<FetchesRecord>();
   public DbSet<FetchesSystem> FetchesSystems => Set<FetchesSystem>();
   #endregion

   #region Posts	
   public DbSet<Category> Categories => Set<Category>();
   public DbSet<CategoryPost> CategoryPosts => Set<CategoryPost>();
   public DbSet<Article> Articles => Set<Article>();
   public DbSet<Attachment> Attachments => Set<Attachment>();
   public DbSet<Reference> References => Set<Reference>();
   public DbSet<Item> Items => Set<Item>();

   public DbSet<Tag> Tags => Set<Tag>();
   public DbSet<TagPost> TagPosts => Set<TagPost>();
   #endregion


   #region Events	
   public DbSet<Tasks> Tasks => Set<Tasks>();
   public DbSet<Event> Events => Set<Event>();
   public DbSet<LocationEvent> LocationEvents => Set<LocationEvent>();
   public DbSet<Calendar> Calendars => Set<Calendar>();
   public DbSet<EventCalendar> EventCalendars => Set<EventCalendar>();
   #endregion
   public override int SaveChanges() => SaveChangesAsync().GetAwaiter().GetResult();

}
