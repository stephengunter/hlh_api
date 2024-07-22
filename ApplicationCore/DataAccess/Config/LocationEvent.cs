using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicationCore.DataAccess.Config;
public class LocationEventConfiguration : IEntityTypeConfiguration<LocationEvent>
{
	public void Configure(EntityTypeBuilder<LocationEvent> builder)
	{
      builder.HasKey(item => new { item.EventId, item.LocationId });

      builder.HasOne<Location>(item => item.Location)
         .WithMany(item => item.LocationEvents)
         .HasForeignKey(item => item.LocationId);


      builder.HasOne<Event>(item => item.Event)
         .WithMany(item => item.LocationEvents)
         .HasForeignKey(item => item.EventId);
   }
}
