using Infrastructure.Entities;

namespace Infrastructure.Views;
public class BaseCategoryView<T> : BaseRecordView where T : BaseCategoryView<T>
{
	public string Title { get; set; } = String.Empty;

   public T? Parent { get; set; }

   public int? ParentId { get; set; }

   public bool IsRootItem { get; set; }

   public ICollection<T> SubItems { get; set; } = new List<T>();
}

