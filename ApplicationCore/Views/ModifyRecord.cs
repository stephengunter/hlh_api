using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Helpers;
using Infrastructure.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ApplicationCore.Views;
public class ModifyRecordViewModel : EntityBaseView
{
   public string EntityType { get; set; } = string.Empty;
   public string EntityId { get; set; } = string.Empty;
   public string Action { get; set; } = string.Empty;
   public DateTime DateTime { get; set; } = DateTime.Now;
   public string UserId { get; set; } = string.Empty;
   public string RemoteIP { get; set; } = string.Empty;

   public string DateTimeText => DateTime.ToDateTimeString();


   public string UserName { get; set; } = string.Empty;
   public ProfilesViewModel? Profiles { get; set; } 
}

