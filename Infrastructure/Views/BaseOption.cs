namespace Infrastructure.Views;
public class BaseOption<Tkey>
{
	public BaseOption(Tkey value, string title)
	{
		this.Value = value;
		this.Title = title;
	}
	public Tkey Value { get; set; }
	public string Title { get; set; }

}

