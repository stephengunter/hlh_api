﻿using Infrastructure.Entities;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models;

public class Tasks : EntityBase, IBaseCategory<Tasks>, IBaseRecord, IRemovable, ISortable
{
  
   public string UserId { get; set; } = String.Empty;

   public string Title { get; set; } = String.Empty;

   public string Content { get; set; } = String.Empty;

   public string? References { get; set; } //Json of Reference

   public DateTime? DeadLine { get; set; }

   public bool Done { get; set; }

   public Tasks? Parent { get; set; }

   public int? ParentId { get; set; }

   public bool IsRootItem => ParentId is null;

   public ICollection<Tasks>? SubItems { get; set; }
   [NotMapped]
   public ICollection<int>? SubIds { get; set; }

   public bool Removed { get; set; }

   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public string CreatedBy { get; set; } = string.Empty;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }

   public int Order { get; set; }

   public bool Active => ISortableHelpers.IsActive(this);

   public void LoadSubItems(IEnumerable<IBaseCategory<Tasks>> tasks) => BaseCategoriesHelpers.LoadSubItems(this, tasks);
}