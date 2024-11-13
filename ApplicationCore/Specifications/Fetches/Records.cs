using ApplicationCore.Models.Fetches;
using Ardalis.Specification;

namespace ApplicationCore.Specifications.Fetches;
public class FetchRecordSpecification : Specification<FetchesRecord>
{
   public FetchRecordSpecification(int year, int month)
   {
      Query.Where(x => x.Year == year && x.Month == month);
   }
   public FetchRecordSpecification(FetchesSystem system, int year, int month)
   {
      Query.Where(x => x.SystemId == system.Id && x.Year == year && x.Month == month);
   }
}

public class FetchRecordMinYearSpecification : Specification<FetchesRecord, int?>
{
   public FetchRecordMinYearSpecification()
   {
      Query.Select(b => (int?)b.Year)
           .OrderBy(b => b.Year)
           .Take(1);
   }
}