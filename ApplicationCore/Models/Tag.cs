﻿using Infrastructure.Entities;
using Infrastructure.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Models;
public class Tag : EntityBase, IBaseRecord, IBaseCategory<Tag>, IRemovable, ISortable
{
   public string Key { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
   public bool Removed { get; set; }
   public DateTime CreatedAt { get; set; } = DateTime.Now;
   public DateTime? LastUpdated { get; set; }
   public string? UpdatedBy { get; set; }
   public int Order { get; set; }
   public bool Active => ISortableHelpers.IsActive(this);

   public Tag? Parent { get; set; }

   [ForeignKey("Parent")]
   public int? ParentId { get; set; }

   public bool IsRootItem => ParentId is null;

   public ICollection<Tag>? SubItems { get; set; }
   public ICollection<int>? SubIds { get; set; }


   public void LoadSubItems(IEnumerable<IBaseCategory<Tag>> tags) => BaseCategoriesHelpers.LoadSubItems(this, tags);

   public virtual ICollection<TagArticle>? TagArticles { get; set; }
}

public class TagArticle
{
   public int TagId { get; set; }
   [Required]
   public virtual Tag? Tag { get; set; }

   public int ArticleId { get; set; }
   [Required]
   public virtual Article? Article { get; set; }
}
