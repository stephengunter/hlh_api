using ApplicationCore.Auth;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Settings;
using ApplicationCore.Helpers;
using ApplicationCore.Views;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using ApplicationCore.Consts;
using ApplicationCore.Specifications;

namespace ApplicationCore.Services;

public interface ITagsService
{
   Task<IEnumerable<Tag>> FetchAllAsync();
   Task<Tag?> GetByIdAsync(int id);
   Task<Tag?> FindByTitleAsync(string title);

   Task<Tag> CreateAsync(Tag tag);
	Task UpdateAsync(Tag tag);
}

public class TagsService : ITagsService
{
	private readonly IDefaultRepository<Tag> _tagsRepository;

	public TagsService(IDefaultRepository<Tag> tagsRepository)
	{
      _tagsRepository = tagsRepository;
	}
   
   public async Task<IEnumerable<Tag>> FetchAllAsync()
      => await _tagsRepository.ListAsync(new TagSpecification());

   public async Task<Tag?> FindByTitleAsync(string title)
      => await _tagsRepository.FirstOrDefaultAsync(new TagSpecification(title));

   public async Task<Tag?> GetByIdAsync(int id)
      => await _tagsRepository.GetByIdAsync(id);

   public async Task<Tag> CreateAsync(Tag Tag)
		=> await _tagsRepository.AddAsync(Tag);

		public async Task UpdateAsync(Tag Tag)
		=> await _tagsRepository.UpdateAsync(Tag);

}
