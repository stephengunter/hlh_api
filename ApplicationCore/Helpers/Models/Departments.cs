using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using System;

namespace ApplicationCore.Helpers;
public static class DepartmentHelpers
{

   public static DepartmentViewModel MapViewModel(this Department department, IMapper mapper)
   {
      var model = mapper.Map<DepartmentViewModel>(department);
      model.SetBaseRecordViewValues();
      
      return model;
   }


   public static List<DepartmentViewModel> MapViewModelList(this IEnumerable<Department> departments, IMapper mapper)
      => departments.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<Department, DepartmentViewModel> GetPagedList(this IEnumerable<Department> departments, IMapper mapper, int page = 1, int pageSize = 999)
   {
      var pageList = new PagedList<Department, DepartmentViewModel>(departments, page, pageSize);
      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }

   public static Department MapEntity(this DepartmentViewModel model, IMapper mapper, string currentUserId, Department? entity = null)
   {
      if (entity == null) entity = mapper.Map<DepartmentViewModel, Department>(model);
      else entity = mapper.Map<DepartmentViewModel, Department>(model, entity);

      entity.SetBaseRecordValues(model);

      if (model.Id == 0) entity.SetCreated(currentUserId);
      else entity.SetUpdated(currentUserId);

      return entity;
   }

   public static IEnumerable<Department> GetOrdered(this IEnumerable<Department> departments)
     => departments.OrderByDescending(item => item.CreatedAt);
}
