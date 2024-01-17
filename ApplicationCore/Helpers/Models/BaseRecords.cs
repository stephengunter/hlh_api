using ApplicationCore.Consts;
using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;
using Infrastructure.Entities;
using Infrastructure.Views;

namespace ApplicationCore.Helpers;
public static class BaseRecordHelpers
{
   public static void SetBaseRecordValues(this BaseRecord entity, BaseRecordView model)
   {
      if (model.Active)
      {
         if (entity.Order < 0) entity.Order = 0;
      }
      else entity.Order = -1;
   }
   public static void SetBaseRecordViewValues(this BaseRecordView model)
   {

      model.CreatedAtText = model.CreatedAt.ToString(DateTimeFormats.Default);
      model.LastUpdatedText = model.LastUpdated.HasValue ?
                                 Convert.ToDateTime(model.LastUpdated).ToString(DateTimeFormats.Default)
                                 : String.Empty;
   }
}
