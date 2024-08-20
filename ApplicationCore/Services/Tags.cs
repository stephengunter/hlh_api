using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Infrastructure.Interfaces;

namespace ApplicationCore.Services;

public interface ITagsService
{
   Task<IEnumerable<Tag>> FetchAsync(string? title);
   Task<Tag?> GetByIdAsync(int id);
   Task<Tag?> FindByTitleAsync(string title);

   Task<Tag> CreateAsync(Tag tag);
	Task UpdateAsync(Tag tag);
   Task DeleteAsync(Tag tag);
}

public class TagsService : ITagsService
{
	private readonly IDefaultRepository<Tag> _tagsRepository;

	public TagsService(IDefaultRepository<Tag> tagsRepository)
	{
      _tagsRepository = tagsRepository;
	}

   public async Task<IEnumerable<Tag>> FetchAsync(string? title)
   {
      if (String.IsNullOrEmpty(title)) return await _tagsRepository.ListAsync();
      else return await _tagsRepository.ListAsync(new TagSpecification(title));
   }

   public async Task<Tag?> FindByTitleAsync(string title)
      => await _tagsRepository.FirstOrDefaultAsync(new TagSpecification(title, true));

   public async Task<Tag?> GetByIdAsync(int id)
      => await _tagsRepository.GetByIdAsync(id);

   public async Task<Tag> CreateAsync(Tag Tag)
		=> await _tagsRepository.AddAsync(Tag);

	public async Task UpdateAsync(Tag Tag)
		=> await _tagsRepository.UpdateAsync(Tag);

   public async Task DeleteAsync(Tag Tag)
      => await _tagsRepository.DeleteAsync(Tag);

}
