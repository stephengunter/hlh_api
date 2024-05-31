using Ardalis.Specification;
using ApplicationCore.Models;
using Infrastructure.Interfaces;
using System.Linq;

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
   public ModifyRecordSpecification(string type, string id, ICollection<string> actions)
   {
      actions = actions.Select(x => x.ToLower()).ToList();
      Query.Where(item => item.EntityType.ToLower() == type.ToLower()
                           && item.EntityId == id
                           && actions.Contains(item.Action.ToLower()));

   }
   public ModifyRecordSpecification(string type, ICollection<string> ids, ICollection<string> actions)
   {
      actions = actions.Select(x => x.ToLower()).ToList();
      Query.Where(item => item.EntityType.ToLower() == type.ToLower()
                           && ids.Contains(item.EntityId)
                           && actions.Contains(item.Action.ToLower()));

   }
}

