using ApplicationCore.Models;
using ApplicationCore.Models.IT;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicationCore.DataAccess.Config;
public class SystemAppDatabaseConfiguration : IEntityTypeConfiguration<SystemAppDatabase>
{
	public void Configure(EntityTypeBuilder<SystemAppDatabase> builder)
	{
      builder.HasKey(item => new { item.SystemAppId, item.DatabaseId });

      builder.HasOne<Database>(item => item.Database)
         .WithMany(item => item.SystemAppDatabases)
         .HasForeignKey(item => item.DatabaseId);


      builder.HasOne<SystemApp>(item => item.SystemApp)
         .WithMany(item => item.SystemAppDatabases)
         .HasForeignKey(item => item.SystemAppId);
   }
}
