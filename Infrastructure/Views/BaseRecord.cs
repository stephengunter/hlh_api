namespace Infrastructure.Views;
public abstract class BaseRecordView
{
	public int Id { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.Now;
	public DateTime? LastUpdated { get; set; }
	public string? UpdatedBy { get; set; }

	public int Order { get; set; }
	public bool Removed { get; set; }

	public bool Active { get; set; }

	public virtual string StatusText => this.Active ? "上架中" : "已下架";

	public string CreatedAtText { get; set; } = string.Empty;

   public string LastUpdatedText { get; set; } = string.Empty;

	public void SetUpdated(string userId)
	{
		UpdatedBy = userId;
		LastUpdated = DateTime.Now;
	}

}

