using Ardalis.Specification;
using ApplicationCore.Models;
using Infrastructure.Interfaces;
using Infrastructure.Helpers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ApplicationCore.Specifications;
public class ModifyRecordSpecification : Specification<ModifyRecord>
{
	public ModifyRecordSpecification(IAggregateRoot entity, string id)
	{
		Query.Where(item => item.EntityType == entity.GetType().Name && item.EntityId == id);
   }
   public ModifyRecordSpecification(string type, string id)
   {
      Query.Where(item => item.EntityType.ToLower() == type.ToLower() && item.EntityId == id);
   }
}

