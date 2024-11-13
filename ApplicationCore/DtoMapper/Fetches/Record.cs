using ApplicationCore.Models.Fetches;
using ApplicationCore.Views.Fetches;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class FetchRecordMappingProfile : Profile
{
	public FetchRecordMappingProfile()
   {
		CreateMap<FetchesRecord, FetchesRecordView>();

		CreateMap<FetchesRecordView, FetchesRecord>();
	}
}
