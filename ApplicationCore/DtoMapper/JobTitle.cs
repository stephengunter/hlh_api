using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class JobTitleMappingProfile : Profile
{
	public JobTitleMappingProfile()
	{
		CreateMap<JobTitle, JobTitleViewModel>();

		CreateMap<JobTitleViewModel, JobTitle>();
	}
}
