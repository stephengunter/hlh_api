using Infrastructure.Helpers;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using Infrastructure.Paging;
using System;

namespace ApplicationCore.Helpers;
public static class TagHelpers
{

   public static TagViewModel MapViewModel(this Tag tag, IMapper mapper)
   {
      var model = mapper.Map<TagViewModel>(tag);
      return model;
   }


   public static List<TagViewModel> MapViewModelList(this IEnumerable<Tag> tags, IMapper mapper)
      => tags.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Tag, TagViewModel> GetPagedList(this IEnumerable<Tag> tags, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Tag, TagViewModel>(tags, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Tag MapEntity(this TagViewModel model, IMapper mapper, string currentUserId, Tag? entity = null)
   {
      if (entity == null) entity = mapper.Map<TagViewModel, Tag>(model);
      else entity = mapper.Map<TagViewModel, Tag>(model, entity);

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);
      return entity;
   }

   public static IEnumerable<Tag> GetOrdered(this IEnumerable<Tag> tags)
     => tags.OrderByDescending(item => item.CreatedAt);
}
