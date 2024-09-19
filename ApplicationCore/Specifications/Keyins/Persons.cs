using ApplicationCore.Models;
using ApplicationCore.Models.Keyin;
using Ardalis.Specification;

namespace ApplicationCore.Specifications.Keyin;
public class KeyinPersonSpecification : Specification<KeyinPerson>
{
   public KeyinPersonSpecification()
   {
      
   }
   public KeyinPersonSpecification(string name)
   {
      Query.Where(x => x.Name == name);
   }
}

public class KeyinAllPassPersonSpecification : Specification<KeyinPerson>
{
   public KeyinAllPassPersonSpecification()
   {
      Query.Where(x => x.AllPass == true);
   }
}

