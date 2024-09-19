using ApplicationCore.Models;
using ApplicationCore.Models.Keyin;
using Ardalis.Specification;

namespace ApplicationCore.Specifications.Keyin;
public class PersonRecordSpecification : Specification<PersonRecord>
{
   public PersonRecordSpecification(int year, int month)
   {
      Query.Where(x => x.Year == year && x.Month == month);
   }
   public PersonRecordSpecification(KeyinPerson person, int year, int month)
   {
      Query.Where(x => x.PersonId == person.Id && x.Year == year && x.Month == month);
   }
}
public class BranchRecordSpecification : Specification<BranchRecord>
{
   public BranchRecordSpecification(int year, int month)
   {
      Query.Where(x => x.BranchId > 0 && x.Year == year && x.Month == month);
   }
   public BranchRecordSpecification(Branch branch, int year, int month)
   {
      Query.Where(x => x.BranchId == branch.Id && x.Year == year && x.Month == month);
   }
}

public class BranchRecordMinYearSpecification : Specification<BranchRecord, int?>
{
   public BranchRecordMinYearSpecification()
   {
      Query.Select(b => (int?)b.Year)
           .OrderBy(b => b.Year)
           .Take(1);
   }
}

public class PersonRecordMinYearSpecification : Specification<PersonRecord, int?>
{
   public PersonRecordMinYearSpecification()
   {
      Query.Select(b => (int?)b.Year)
           .OrderBy(b => b.Year)
           .Take(1);
   }
}

