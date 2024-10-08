using ApplicationCore.Views;
using ApplicationCore.Models;
using Infrastructure.Helpers;
using AutoMapper;
using Infrastructure.Paging;

namespace ApplicationCore.Helpers;

public static class UsersHelpers
{
   public static string GetUserName(this User user)
      => String.IsNullOrEmpty(user.UserName) ? String.Empty : user.UserName;


   public static IEnumerable<User> GetOrdered(this IEnumerable<User> users)
      => users.OrderByDescending(u => u.CreatedAt);

   public static IEnumerable<User> FilterByKeyword(this IEnumerable<User> users, ICollection<string> keywords)
   {
      var byUsername = users.FilterByUsername(keywords);
      var byName = users.FilterByName(keywords);
      return byUsername.Union(byName, new UserEqualityComparer()).ToList();
   }

   public static IEnumerable<User> FilterByUsername(this IEnumerable<User> users, ICollection<string> keywords)
      => users.Where(user => keywords.Any(user.GetUserName().CaseInsensitiveContains)).ToList();

   public static IEnumerable<User> FilterByName(this IEnumerable<User> users, ICollection<string> keywords)
      => users.Where(user => keywords.Any(user.Name.CaseInsensitiveContains)).ToList();


   #region Views
   public static UserViewModel MapViewModel(this User user, IMapper mapper, Department? department = null)
   {
      var model = mapper.Map<UserViewModel>(user);
      if (user.Profiles != null) model.Profiles = user.Profiles.MapViewModel(mapper, department);
      if (user.UserRoles!.HasItems()) model.Roles = user.UserRoles!.Select(x => x.RoleId).JoinToString(); 
      return model;
   }
   public static User MapEntity(this UserViewModel model, IMapper mapper, string currentUserId, User? entity = null)
   {
      if (entity == null) entity = mapper.Map<UserViewModel, User>(model);
      else entity = mapper.Map<UserViewModel, User>(model, entity);
      
      entity.LastUpdated = DateTime.Now;

      return entity;
   }
   public static User MapEntity(this UserViewModel model, IMapper mapper)
      => mapper.Map<UserViewModel, User>(model);

   public static List<UserViewModel> MapViewModelList(this IEnumerable<User> users, IMapper mapper)
      => users.Select(item => MapViewModel(item, mapper)).ToList();

   public static PagedList<User, UserViewModel> GetPagedList(this IEnumerable<User> users, IMapper mapper, int page = 1, int pageSize = -1)
   {
      var pageList = new PagedList<User, UserViewModel>(users, page, pageSize);

      pageList.SetViewList(pageList.List.MapViewModelList(mapper));

      return pageList;
   }
   #endregion

}

public class UserEqualityComparer : IEqualityComparer<User>
{
   public bool Equals(User? a, User? b) => a!.Id == b!.Id;

   public int GetHashCode(User obj) => obj.Id.GetHashCode() ^ obj.UserName!.GetHashCode();
}

public static class RolesHelpers
{
   public static RoleViewModel MapViewModel(this Role role, IMapper mapper)
      => mapper.Map<RoleViewModel>(role);

   public static List<RoleViewModel> MapViewModelList(this IEnumerable<Role> roles, IMapper mapper)
      => roles.Select(item => MapViewModel(item, mapper)).ToList();
}