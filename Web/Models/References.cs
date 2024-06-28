namespace Web.Models
{
   public class ReferenceCreateForm
   {
      public string Title { get; set; } = String.Empty;
      public string? Url { get; set; } = String.Empty;
      public IFormFile? File { get; set; }

   }
}
