using ApplicationCore.Models.Keyin;
using ApplicationCore.Views.Keyin;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class BranchRecordMappingProfile : Profile
{
	public BranchRecordMappingProfile()
	{
		CreateMap<BranchRecord, BranchRecordView>();

		CreateMap<BranchRecordView, BranchRecord>();
	}
}
