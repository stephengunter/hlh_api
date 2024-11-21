using ApplicationCore.Models.IT;
using Ardalis.Specification;
using Infrastructure.Helpers;

namespace ApplicationCore.Specifications;
public class CredentialInfoSpecification : Specification<CredentialInfo>
{
   public CredentialInfoSpecification()
   {
      Query.Where(item => !item.Removed);
   }
   public CredentialInfoSpecification(Host host)
   {
      Query.Where(item => !item.Removed && item.EntityType == nameof(Host) && item.EntityId == host.Id);
   }
   public CredentialInfoSpecification(Server server)
   {
      Query.Where(item => !item.Removed && item.EntityType == nameof(Server) && item.EntityId == server.Id);
   }
}
