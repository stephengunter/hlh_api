using Infrastructure.Entities;
using Infrastructure.Helpers;

namespace ApplicationCore.Models;

public class TelName : EntityBase
{
   public string Department { get; set; } = string.Empty;

   public string Role { get; set; } = string.Empty;

   public string Tel { get; set; } = string.Empty;

   public string Name { get; set; } = string.Empty;

   public string Ad { get; set; } = string.Empty;
}


