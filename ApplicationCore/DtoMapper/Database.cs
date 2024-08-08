using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class CalendarMappingProfile : Profile
{
	public CalendarMappingProfile()
	{
		CreateMap<Calendar, CalendarViewModel>();

		CreateMap<CalendarViewModel, Calendar>();
	}
}
