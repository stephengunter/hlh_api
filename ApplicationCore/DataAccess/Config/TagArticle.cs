using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApplicationCore.DataAccess.Config;
public class TagArticleConfiguration : IEntityTypeConfiguration<TagArticle>
{
	public void Configure(EntityTypeBuilder<TagArticle> builder)
	{
      builder.HasKey(item => new { item.TagId, item.ArticleId });

      builder.HasOne<Tag>(item => item.Tag)
         .WithMany(item => item.TagArticles)
         .HasForeignKey(item => item.TagId);


      builder.HasOne<Article>(item => item.Article)
         .WithMany(item => item.TagArticles)
         .HasForeignKey(item => item.ArticleId);
   }
}
