using ApplicationCore.Consts;
using Infrastructure.Entities;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models;

public class Branch : EntityBase, IRemovable, ISortable
{
   public string Key { get; set; } = String.Empty;

   public string Title { get; set; } = String.Empty;

   public bool Removed { get; set; }
   public int Order { get; set; }

   public bool Active => ISortableHelpers.IsActive(this);

}


