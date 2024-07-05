﻿using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using ApplicationCore.Models.Auth;

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

   #region Posts	
   public DbSet<Category> Categories => Set<Category>();
   public DbSet<CategoryPost> CategoryPosts => Set<CategoryPost>();
   public DbSet<Article> Articles => Set<Article>();
   public DbSet<Attachment> Attachments => Set<Attachment>();
   public DbSet<Reference> References => Set<Reference>();

   public DbSet<Tag> Tags => Set<Tag>();
   public DbSet<TagPost> TagPosts => Set<TagPost>();
   #endregion

   public DbSet<Tasks> Tasks => Set<Tasks>();

   public DbSet<Event> Events => Set<Event>();
   public DbSet<LocationEvent> LocationEvents => Set<LocationEvent>();
   public DbSet<Calendar> Calendars => Set<Calendar>();
   public DbSet<EventCalendar> EventCalendars => Set<EventCalendar>();

   public override int SaveChanges() => SaveChangesAsync().GetAwaiter().GetResult();

}
