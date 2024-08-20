using ApplicationCore.Models.IT;
using ApplicationCore.Views.IT;
using AutoMapper;
using Infrastructure.Paging;
using Infrastructure.Helpers;

namespace ApplicationCore.Helpers;
public static class CredentialInfosHelpers
{

   public static CredentialInfoViewModel MapViewModel(this CredentialInfo entity, IMapper mapper)
   {
      var model = mapper.Map<CredentialInfoViewModel>(entity);
      return model;
   }

   public static List<CredentialInfoViewModel> MapViewModelList(this IEnumerable<CredentialInfo> entities, IMapper mapper)
      => entities.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<CredentialInfo, CredentialInfoViewModel> GetPagedList(this IEnumerable<CredentialInfo> entities, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<CredentialInfo, CredentialInfoViewModel>(entities, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static CredentialInfo MapEntity(this CredentialInfoViewModel model, IMapper mapper, string currentUserId, CredentialInfo? entity = null)
   {
      if (entity == null) entity = mapper.Map<CredentialInfoViewModel, CredentialInfo>(model);
      else entity = mapper.Map<CredentialInfoViewModel, CredentialInfo>(model, entity);

      entity.SetActive(model.Active);

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);

      return entity;
   }

   public static IEnumerable<CredentialInfo> GetOrdered(this IEnumerable<CredentialInfo> calendars)
     => calendars.OrderBy(item => item.Order);
}
