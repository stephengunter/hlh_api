using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class ModifyRecordMappingProfile : Profile
{
	public ModifyRecordMappingProfile()
	{
		CreateMap<ModifyRecord, ModifyRecordViewModel>();

		CreateMap<ModifyRecordViewModel, ModifyRecord>();
	}
}
