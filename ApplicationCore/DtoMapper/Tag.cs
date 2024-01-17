using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class TagMappingProfile : AutoMapper.Profile
{
	public TagMappingProfile()
	{
		CreateMap<Tag, TagViewModel>();

		CreateMap<TagViewModel, Tag>();
	}
}
