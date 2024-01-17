using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class AttachmentMappingProfile : AutoMapper.Profile
{
	public AttachmentMappingProfile()
	{
		CreateMap<UploadFile, AttachmentViewModel>();

		CreateMap<AttachmentViewModel, UploadFile>();
	}
}
