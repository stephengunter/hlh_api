using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class ItemMappingProfile : Profile
{
	public ItemMappingProfile()
	{
		CreateMap<Item, ItemViewModel>();

		CreateMap<ItemViewModel, Item>();
	}
}
