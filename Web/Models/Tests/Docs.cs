using ApplicationCore.Models;
using ApplicationCore.Views;
using ApplicationCore.Views.Judicial;
using Infrastructure.Paging;

namespace Web.Models;
public class DocsIndexModel
{
   public List<UnitPerson> UnitPersons { get; set; } = new List<UnitPerson>();

}

public class DocKeepRequest
{
   public int Flag { get; set; }
   public List<DocModel> DocModels { get; set; } = new List<DocModel>();
   public string Person { get; set; } = string.Empty;
}

public class DocsFetchRequest
{
   public DocsFetchRequest(int flag, string person)
   {
      Flag = flag;
      Person = person;
   }

   public int Flag { get; set; }
   public string Person { get; set; } = string.Empty;
}

public class DocsKeppRequest
{
   public int Flag { get; set; }
   public List<int> Ids { get; set; } = new List<int>();
   public string Person { get; set; } = string.Empty;
}