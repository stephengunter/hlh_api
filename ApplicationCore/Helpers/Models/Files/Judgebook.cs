using Infrastructure.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views.Files;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Views;
using System;
using ApplicationCore.Models.Files;
using Ardalis.Specification;
using Microsoft.IdentityModel.Tokens;
using ApplicationCore.Consts;

namespace ApplicationCore.Helpers.Files;
public static class JudgebookFileHelpers
{
   public static string CourtTypeTitle(this string val)
   {
      string value = val.ToUpper();
      if (value == JudgeCourtTypes.H) return "刑事";
      if (value == JudgeCourtTypes.V) return "民事";
      return "";
   }
   public static string OriginTypeTitle(this string val)
   {
      string value = val.ToUpper();
      if (value == OriginTypes.M) return "原本";
      if (value == OriginTypes.O) return "正本";
      return "";
   }
   public static JudgebookFileViewModel MapViewModel(this JudgebookFile entity, IMapper mapper, string fileFullPath = "")
   {
      var model = mapper.Map<JudgebookFileViewModel>(entity);
      if (!String.IsNullOrEmpty(fileFullPath))
      {
         model.FileView = new BaseFileView(entity.FileName, File.ReadAllBytes(fileFullPath));
      }
      return model;
   }
   public static JudgebookFileViewModel MapViewModel(this JudgebookFile entity, IMapper mapper, byte[] filebytes)
   {
      var model = mapper.Map<JudgebookFileViewModel>(entity);
      model.FileView = new BaseFileView(entity.FileName, filebytes);
      return model;
   }
   public static JudgebookFileReportItem MapReportItem(this JudgebookFile entity, IMapper mapper)
   {
      var item = new JudgebookFileReportItem();
      entity.SetValuesTo(item);
      item.TypeTitle = entity.Type.Title;

      return item;
   }

   public static List<JudgebookFileViewModel> MapViewModelList(this IEnumerable<JudgebookFile> entities, IMapper mapper)
      => entities.Select(item => MapViewModel(item, mapper)).ToList();

   public static List<JudgebookFileReportItem> MapReportItemList(this IEnumerable<JudgebookFile> entities, IMapper mapper)
      => entities.Select(item => MapReportItem(item, mapper)).ToList();

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

   public static bool IsSameCase(this JudgebookFile entry, IJudgebookFile model)
   { 
      return (entry.TypeId == model.TypeId) && (entry.CourtType == model.CourtType) && (entry.Year == model.Year) 
         && (entry.Category == model.Category) && (entry.Num == model.Num);
   }

   public static Dictionary<string, string> Validate(this JudgebookFile model)
   {
      var errors = new Dictionary<string, string>();
    
      if (model.TypeId < 1) errors.Add("type", "錯誤的type");
      if (string.IsNullOrEmpty(model.CourtType)) errors.Add("courtType", "錯誤的courtType");
      if (string.IsNullOrEmpty(model.Year)) errors.Add("year", "錯誤的年度");
      if (string.IsNullOrEmpty(model.Category)) errors.Add("category", "錯誤的字號");
      if (string.IsNullOrEmpty(model.Num)) errors.Add("num", "錯誤的案號");

      return errors;
   }
}
