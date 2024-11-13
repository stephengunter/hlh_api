using ApplicationCore.Models;
using ApplicationCore.Models.Criminals;
using ApplicationCore.Models.Keyin;
using Ardalis.Specification;

namespace ApplicationCore.Specifications.Criminals;
public class CriminalFetchRecordSpecification : Specification<CriminalFetchRecord>
{
   public CriminalFetchRecordSpecification(int year, int month)
   {
      Query.Where(x => x.Year == year && x.Month == month);
   }
}

public class CriminalFetchRecordMinYearSpecification : Specification<CriminalFetchRecord, int?>
{
   public CriminalFetchRecordMinYearSpecification()
   {
      Query.Select(b => (int?)b.Year)
           .OrderBy(b => b.Year)
           .Take(1);
   }
}