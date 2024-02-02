using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models;
public class Tag : BaseCategory<Tag>
{
   public string Key { get; set; } = String.Empty;
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
