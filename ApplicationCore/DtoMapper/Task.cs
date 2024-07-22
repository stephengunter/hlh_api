using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class TaskMappingProfile : Profile
{
	public TaskMappingProfile()
	{
		CreateMap<Tasks, TaskViewModel>();

		CreateMap<TaskViewModel, Tasks>();
	}
}
