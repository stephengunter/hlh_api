using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class JobMappingProfile : AutoMapper.Profile
{
	public JobMappingProfile()
	{
		CreateMap<Job, JobViewModel>();

		CreateMap<JobViewModel, Job>();
	}
}
