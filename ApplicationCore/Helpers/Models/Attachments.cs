using Infrastructure.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using Infrastructure.Paging;
using System;
using Ardalis.Specification;
using Infrastructure.Views;
using Infrastructure.Consts;

namespace ApplicationCore.Helpers;
public static class AttachmentsHelpers
{
   public static AttachmentViewModel MapViewModel(this Attachment attachment, IMapper mapper)
   {
      var model = mapper.Map<AttachmentViewModel>(attachment);
      model.FileType = attachment.Ext.GetFileType().ToString();
      return model;
   }

   public static AttachmentViewModel MapViewModel(this Attachment attachment, IMapper mapper, byte[] filebytes)
   {
      var model = MapViewModel(attachment, mapper);
      model.FileView = new BaseFileView(attachment.FileName, filebytes);
      return model;
   }


   public static List<AttachmentViewModel> MapViewModelList(this IEnumerable<Attachment> attachments, IMapper mapper)
      => attachments.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Attachment, AttachmentViewModel> GetPagedList(this IEnumerable<Attachment> attachments, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Attachment, AttachmentViewModel>(attachments, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Attachment MapEntity(this AttachmentViewModel model, IMapper mapper, string currentUserId, Attachment? entity = null)
   {
      if (entity == null) entity = mapper.Map<AttachmentViewModel, Attachment>(model);
      else entity = mapper.Map<AttachmentViewModel, Attachment>(model, entity);

      entity.SetActive(model.Active);

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);

      return entity;
   }

   public static IEnumerable<Attachment> GetOrdered(this IEnumerable<Attachment> attachments)
     => attachments.OrderByDescending(item => item.CreatedAt);
}
