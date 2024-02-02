using Infrastructure.Entities;

namespace ApplicationCore.Models;
public class Category : BaseCategory<Category>
{
    public string Key { get; set; } = String.Empty;
}
