using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services;

public interface IAttachmentService
{
   Task<IEnumerable<Attachment>> FetchAsync();
   Task<Attachment?> GetByIdAsync(int id);

   Task<Attachment> CreateAsync(Attachment entity);
	Task UpdateAsync(Attachment entity);
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

   public async Task<Attachment?> GetByIdAsync(int id)
      => await _attachmentRepository.GetByIdAsync(id);

   public async Task<Attachment> CreateAsync(Attachment entity)
		=> await _attachmentRepository.AddAsync(entity);

		public async Task UpdateAsync(Attachment entity)
		=> await _attachmentRepository.UpdateAsync(entity);

}
