using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicationCore.DataAccess.Config;
public class EventCalendarConfiguration : IEntityTypeConfiguration<EventCalendar>
{
	public void Configure(EntityTypeBuilder<EventCalendar> builder)
	{
      builder.HasKey(item => new { item.EventId, item.CalendarId });

      builder.HasOne<Calendar>(item => item.Calendar)
         .WithMany(item => item.EventCalendars)
         .HasForeignKey(item => item.CalendarId);


      builder.HasOne<Event>(item => item.Event)
         .WithMany(item => item.EventCalendars)
         .HasForeignKey(item => item.EventId);
   }
}
