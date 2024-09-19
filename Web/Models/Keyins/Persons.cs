using ApplicationCore.Models;
using ApplicationCore.Views;
using ApplicationCore.Views.Keyin;
using Infrastructure.Paging;

namespace Web.Models.Keyin;

public class PassesPersonUploadRequest
{
   public IFormFile? File { get; set; }
}

public class PassesPersonAddRequest
{
   public ICollection<KeyinPersonView> Persons { get; set; } = new List<KeyinPersonView>();
}