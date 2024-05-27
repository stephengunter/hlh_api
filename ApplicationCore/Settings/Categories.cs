namespace ApplicationCore.Settings;

public class CategoriesSettings
{
   public string Key { get; set; } = string.Empty;
   public string Title { get; set; } = string.Empty;
   public ICollection<CategoriesSettings> Categories { get; set; } = new List<CategoriesSettings>();
}
public class EventSettings
{
   public string Key { get; set; } = string.Empty;

   public ICollection<CategoriesSettings> Categories { get; set; } = new List<CategoriesSettings>();
}



