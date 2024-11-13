using AutoMapper;
using Infrastructure.Paging;
using ApplicationCore.Models.Fetches;
using ApplicationCore.Views.Fetches;

namespace ApplicationCore.Helpers.Fetches;
public static class FetchesSystemHelpers
{
   public static FetchesSystemView MapViewModel(this FetchesSystem record, IMapper mapper)
   {
      var model = mapper.Map<FetchesSystemView>(record);
      return model;
   }

   public static List<FetchesSystemView> MapViewModelList(this IEnumerable<FetchesSystem> records, IMapper mapper)
      => records.Select(item => MapViewModel(item, mapper)).ToList();

   public static FetchesSystem MapEntity(this FetchesSystemView model, IMapper mapper, FetchesSystem? entity = null)
   {
      if (entity == null) entity = mapper.Map<FetchesSystemView, FetchesSystem>(model);
      else entity = mapper.Map<FetchesSystemView, FetchesSystem>(model, entity);

      return entity;
   }

   public static IEnumerable<FetchesSystem> GetOrdered(this IEnumerable<FetchesSystem> records)
    => records.OrderBy(item => item.Department);

}
