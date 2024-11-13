using AutoMapper;
using Infrastructure.Paging;
using ApplicationCore.Models.Fetches;
using ApplicationCore.Views.Fetches;
using System.Text.RegularExpressions;
using Infrastructure.Helpers;

namespace ApplicationCore.Helpers.Fetches;
public static class FetchesRecordHelpers
{
   public static FetchesRecordView MapViewModel(this FetchesRecord record, IMapper mapper)
   {
      var model = mapper.Map<FetchesRecordView>(record);
      model.QueryTime = $"{record.Year}/{record.Month}/{record.Day} {record.Time}";
      return model;
   }


   public static List<FetchesRecordView> MapViewModelList(this IEnumerable<FetchesRecord> records, IMapper mapper)
      => records.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<FetchesRecord, FetchesRecordView> GetPagedList(this IEnumerable<FetchesRecord> records, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<FetchesRecord, FetchesRecordView>(records, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static FetchesRecord MapEntity(this FetchesRecordView model, IMapper mapper, FetchesRecord? entity = null)
   {
      if (entity == null) entity = mapper.Map<FetchesRecordView, FetchesRecord>(model);
      else entity = mapper.Map<FetchesRecordView, FetchesRecord>(model, entity);
      
      var datetime = ParseQueryTime(model.QueryTime);
      if (datetime.Count() == 4)
      {
         entity.Year = datetime[0].ToInt();
         entity.Month = datetime[1].ToInt();
         entity.Day = datetime[2].ToInt();
         entity.Time = datetime[3];
      }

      return entity;
   }
   public static List<FetchesRecord> MapEntityList(this IEnumerable<FetchesRecordView> views, IMapper mapper)
     => views.Select(item => MapEntity(item, mapper)).ToList();

   public static IEnumerable<FetchesRecord> GetOrdered(this IEnumerable<FetchesRecord> records)
    => records.OrderBy(item => item.Day).OrderBy(item => item.Id);

   static string[] ParseQueryTime(string queryTime)
   {
      // Regex pattern to match '113/08/20(14:25:36)'
      string pattern = @"(\d{3})/(\d{2})/(\d{2})\((\d{2}:\d{2}:\d{2})\)";

      // Use Regex to extract parts
      var match = Regex.Match(queryTime, pattern);

      if (match.Success)
      {
         // Return the captured groups as an array
         return new string[]
         {
                match.Groups[1].Value,  // Year (113)
                match.Groups[2].Value.TrimStart('0'),  // Month (08, trim leading zero)
                match.Groups[3].Value.TrimStart('0'),  // Day (20, trim leading zero)
                match.Groups[4].Value   // Time (14:25:36)
         };
      }

      // If the input format is incorrect, return an empty array or handle it accordingly
      return new string[0];
   }

}
