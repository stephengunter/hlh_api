using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Helpers;
using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.Models;
public class ModifyRecord : EntityBase, IModifyRecord
{
   public ModifyRecord()
   { 
   
   }
   public ModifyRecord(IAggregateRoot entity, string action, string userId, DateTime dateTime)
   {
      EntityType = entity.GetType().Name;
      EntityId = entity.GetId().ToString()!;
      Action = action;
      UserId = userId;
      DateTime = dateTime;
   }
   public string EntityType { get; set; } = string.Empty;
   public string EntityId { get; set; } = string.Empty;
   public string Action { get; set; } = string.Empty;
   public DateTime DateTime { get; set; } = DateTime.Now;
   public string UserId { get; set; } = string.Empty;
}

