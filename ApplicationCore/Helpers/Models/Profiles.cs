using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using System;

namespace ApplicationCore.Helpers;
public static class ProfilesHelpers
{
   public static ProfilesViewModel MapViewModel(this Profiles profiles, IMapper mapper)
      => mapper.Map<ProfilesViewModel>(profiles);

   public static Profiles MapEntity(this ProfilesViewModel model, IMapper mapper, Profiles? entity = null)
   {
      if (entity == null) entity = mapper.Map<ProfilesViewModel, Profiles>(model);
      else entity = mapper.Map<ProfilesViewModel, Profiles>(model, entity);

      return entity;
   }
}
