using Infrastructure.Consts;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Views;
using System.Runtime.Serialization;

namespace Infrastructure.Helpers;
public static class BaseRecordHelpers
{
   public static void SetCreated(this IBaseRecord entity, string updatedBy)
   {
      entity.CreatedAt = DateTime.Now;
      SetUpdated(entity, updatedBy);
   }
   public static void SetUpdated(this IBaseRecord entity, string updatedBy)
   {
      entity.UpdatedBy = updatedBy;
      entity.LastUpdated = DateTime.Now;
   }
}
