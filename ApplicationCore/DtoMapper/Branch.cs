using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class BranchMappingProfile : Profile
{
	public BranchMappingProfile()
	{
		CreateMap<Branch, BranchViewModel>();

		CreateMap<BranchViewModel, Branch>();
	}
}
