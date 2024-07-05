using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Infrastructure.Entities;

namespace ApplicationCore.Services;

public interface IReferenceService
{
   Task<IEnumerable<Reference>> FetchAsync();
   Task<IEnumerable<Reference>> FetchAsync(string postType, int postId);
   Task<IEnumerable<Reference>> FetchAsync(EntityBase entity);
   Task<Reference?> GetByIdAsync(int id);

   Task<Reference> CreateAsync(Reference entity);
	Task UpdateAsync(Reference entity);
}

public class ReferenceService : IReferenceService
{
	private readonly IDefaultRepository<Reference> _referenceRepository;

	public ReferenceService(IDefaultRepository<Reference> referenceRepository)
	{
      _referenceRepository = referenceRepository;
	}

   public async Task<IEnumerable<Reference>> FetchAsync()
      => await _referenceRepository.ListAsync(new ReferencesSpecification());

   public async Task<IEnumerable<Reference>> FetchAsync(EntityBase entity)
   {
      string type = entity.GetType().Name;
      return await _referenceRepository.ListAsync(new ReferencesSpecification(type, entity.Id));
   }

   public async Task<IEnumerable<Reference>> FetchAsync(string postType, int postId)
      => await _referenceRepository.ListAsync(new ReferencesSpecification(postType, postId));


   public async Task<Reference?> GetByIdAsync(int id)
      => await _referenceRepository.GetByIdAsync(id);

   public async Task<Reference> CreateAsync(Reference entity)
		=> await _referenceRepository.AddAsync(entity);

		public async Task UpdateAsync(Reference entity)
		=> await _referenceRepository.UpdateAsync(entity);

}
