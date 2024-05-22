using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Helpers;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Newtonsoft.Json;

namespace ApplicationCore.Models;
public class ModifyRecord : EntityBase, IModifyRecord
{
   public ModifyRecord()
   { 
   
   }
   public ModifyRecord(IAggregateRoot entity, string action, string userId, string ip, DateTime dateTime, string dataJson)
   {
      EntityType = entity.GetType().Name;
      EntityId = entity.GetId().ToString()!;
      Action = action;
      UserId = userId;
      RemoteIP = ip;
      DateTime = dateTime;
      DataJson = dataJson;
   }
   public static ModifyRecord Create(IAggregateRoot entity, string action, string userId, string ip)
   {
      string json = JsonConvert.SerializeObject(entity, Formatting.None,
                        new JsonSerializerSettings()
                        {
                           ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });
      return new ModifyRecord(entity, action, userId, ip, DateTime.Now, json);
      
   }
   public string EntityType { get; set; } = string.Empty;
   public string EntityId { get; set; } = string.Empty;
   public string Action { get; set; } = string.Empty;
   public DateTime DateTime { get; set; } = DateTime.Now;
   public string UserId { get; set; } = string.Empty;

   public string RemoteIP { get; set; } = string.Empty;
   public string DataJson { get; set; } = string.Empty;
}

