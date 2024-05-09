using Infrastructure.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views.Files;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Views;
using System;
using ApplicationCore.Models.Files;
using Ardalis.Specification;

namespace ApplicationCore.Helpers.Files;
public static class JudgebookFileHelpers
{
   public static JudgebookFileViewModel MapViewModel(this JudgebookFile entity, IMapper mapper, string fileFullPath = "")
   {
      var model = mapper.Map<JudgebookFileViewModel>(entity);
      if (!String.IsNullOrEmpty(fileFullPath))
      {
         model.FileView = new BaseFileView()
         {
            FileName = entity.FileName,
            FileBytes = File.ReadAllBytes(fileFullPath)
         };
      }
      return model;
   }
   public static JudgebookFileViewModel MapViewModel(this JudgebookFile entity, IMapper mapper, byte[] filebytes)
   {
      var model = mapper.Map<JudgebookFileViewModel>(entity);
      model.FileView = new BaseFileView()
      {
         FileName = entity.FileName,
         FileBytes = filebytes
      };
      return model;
   }


   public static List<JudgebookFileViewModel> MapViewModelList(this IEnumerable<JudgebookFile> entities, IMapper mapper)
      => entities.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<JudgebookFile, JudgebookFileViewModel> GetPagedList(this IEnumerable<JudgebookFile> entities, IMapper mapper,
      int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<JudgebookFile, JudgebookFileViewModel>(entities, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static JudgebookFile MapEntity(this JudgebookFileViewModel model, IMapper mapper, string currentUserId, JudgebookFile? entity = null)
   {
      if (entity == null) entity = mapper.Map<JudgebookFileViewModel, JudgebookFile>(model);
      else entity = mapper.Map<JudgebookFileViewModel, JudgebookFile>(model, entity);

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);

      return entity;
   }

   public static IEnumerable<JudgebookFile> GetOrdered(this IEnumerable<JudgebookFile> entities)
     => entities.OrderByDescending(item => item.CreatedAt);

   public static string CreateFileName(this JudgebookFile entry)
   { 
      if (entry.Removed) return $"{entry.CourtType}_{entry.Year}_{entry.Category}_{entry.Num}_{entry.Type.Key}";
      return $"{entry.Year}_{entry.Category}_{entry.Num}_{entry.Type.Key}";
   }

   public static bool IsSameCase(this JudgebookFile entry, JudgebookFileViewModel model)
   { 
      return (entry.TypeId == model.TypeId) && (entry.CourtType == model.CourtType) && (entry.Year == model.Year) 
         && (entry.Category == model.Category) && (entry.Num == model.Num);
   }
}
