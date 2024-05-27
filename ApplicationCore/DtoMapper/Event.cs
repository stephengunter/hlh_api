using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class EventMappingProfile : Profile
{
	public EventMappingProfile()
	{
		CreateMap<Event, EventViewModel>();

		CreateMap<EventViewModel, Event>();
	}
}
