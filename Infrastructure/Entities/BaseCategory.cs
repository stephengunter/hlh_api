using Infrastructure.Helpers;
using Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Entities;

public abstract class BaseCategory<T> : BaseRecord where T : BaseCategory<T>
{
	public string Title { get; set; } = String.Empty;

	public virtual T? Parent { get; set; }

   [ForeignKey("Parent")]
   public int? ParentId { get; set; }

	public bool IsRootItem => ParentId is null;

   public ICollection<T> SubItems { get; set; } = new List<T>();

   
   public void LoadSubItems(IEnumerable<BaseCategory<T>> categories)
   {
      SubItems = categories.Where(item => item.ParentId == this.Id).Select(item => (T)item).ToList();
      if(SubItems.HasItems()) SubIds.AddRangeIfNotExists(SubItems.Select(c => c.Id));

      foreach (var item in SubItems) item.LoadSubItems(categories);
   }
   [NotMapped]
   public ICollection<int> SubIds { get; private set; } = new List<int>();

}


