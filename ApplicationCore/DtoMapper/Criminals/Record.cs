using ApplicationCore.Models.Criminals;
using ApplicationCore.Views.Criminals;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class CriminalFetchRecordMappingProfile : Profile
{
	public CriminalFetchRecordMappingProfile()
	{
		CreateMap<CriminalFetchRecord, CriminalFetchRecordView>();

		CreateMap<CriminalFetchRecordView, CriminalFetchRecord>();
	}
}
