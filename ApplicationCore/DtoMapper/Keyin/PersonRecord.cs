using ApplicationCore.Models.Keyin;
using ApplicationCore.Views.Keyin;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class PersonRecordMappingProfile : Profile
{
	public PersonRecordMappingProfile()
	{
		CreateMap<PersonRecord, PersonRecordView>();

		CreateMap<PersonRecordView, PersonRecord>();
	}
}
