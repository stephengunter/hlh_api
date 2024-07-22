using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Ardalis.Specification;
using Infrastructure.Entities;
using Infrastructure.Helpers;

namespace ApplicationCore.Services;

public interface IAttachmentService
{
   Task<IEnumerable<Attachment>> FetchAsync();
   Task<IEnumerable<Attachment>> FetchAsync(string postType, int postId);
   Task<IEnumerable<Attachment>> FetchAsync(EntityBase entity);
   Task<IEnumerable<Attachment>> FetchAsync(ICollection<int> ids);
   Task<Attachment?> GetByIdAsync(int id);

   Task<Attachment> CreateAsync(Attachment entity);
	Task UpdateAsync(Attachment entity);
   Task UpdateRangeAsync(IEnumerable<Attachment> attachments);
   Task RemoveAsync(Attachment entity, string userId);
   Task RemoveRangeAsync(IEnumerable<Attachment> attachments, string userId);
}

public class AttachmentService : IAttachmentService
{
	private readonly IDefaultRepository<Attachment> _attachmentRepository;

	public AttachmentService(IDefaultRepository<Attachment> attachmentRepository)
	{
      _attachmentRepository = attachmentRepository;
	}

   public async Task<IEnumerable<Attachment>> FetchAsync()
      => await _attachmentRepository.ListAsync(new AttachmentsSpecification());
   public async Task<IEnumerable<Attachment>> FetchAsync(ICollection<int> ids)
      => await _attachmentRepository.ListAsync(new AttachmentsSpecification(ids));

   public async Task<IEnumerable<Attachment>> FetchAsync(EntityBase entity)
      => await _attachmentRepository.ListAsync(new AttachmentsByTypesSpecification(entity.GetType().Name, entity.Id));

   public async Task<IEnumerable<Attachment>> FetchAsync(string postType, int postId)
      => await _attachmentRepository.ListAsync(new AttachmentsByTypesSpecification(postType, postId));

   public async Task<Attachment?> GetByIdAsync(int id)
      => await _attachmentRepository.GetByIdAsync(id);

   public async Task<Attachment> CreateAsync(Attachment entity)
		=> await _attachmentRepository.AddAsync(entity);

	public async Task UpdateAsync(Attachment entity)
		=> await _attachmentRepository.UpdateAsync(entity); 
   
   public async Task UpdateRangeAsync(IEnumerable<Attachment> attachments)
      => await _attachmentRepository.UpdateRangeAsync(attachments);

   public async Task RemoveAsync(Attachment entity, string userId)
   {
      entity.Removed = true;
      entity.SetUpdated(userId);
      await _attachmentRepository.UpdateAsync(entity);
   }

   public async Task RemoveRangeAsync(IEnumerable<Attachment> attachments, string userId)
   {
      foreach (var entity in attachments)
      {
         entity.Removed = true;
         entity.SetUpdated(userId);
      }
      await _attachmentRepository.UpdateRangeAsync(attachments);
   }

}
