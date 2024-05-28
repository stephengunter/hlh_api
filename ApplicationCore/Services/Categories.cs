using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ApplicationCore.Services;

public interface ICategorysService
{
   Task<IEnumerable<Category>> FindRootAsync(PostType type);
   
   Task<IEnumerable<Category>> FetchByKeysAsync(PostType type, IList<string> keys);
   Task<IEnumerable<Category>> FetchAllAsync();

   Task<Category?> GetByKeyAsync(PostType type, string key);
   Task<Category?> GetByIdAsync(int id);

   Task<Category> CreateAsync(Category Category);
	Task UpdateAsync(Category Category);
}

public class CategorysService : ICategorysService
{
	private readonly IDefaultRepository<Category> _categorysRepository;

	public CategorysService(IDefaultRepository<Category> categorysRepository)
	{
      _categorysRepository = categorysRepository;
	}
   public async Task<IEnumerable<Category>> FindRootAsync(PostType type)
       => await _categorysRepository.ListAsync(new RootCategoriesSpecification(type));
   public async Task<Category?> GetByKeyAsync(PostType type, string key)
       => await _categorysRepository.FirstOrDefaultAsync(new CategoriesSpecification(type, key));

   public async Task<IEnumerable<Category>> FetchByKeysAsync(PostType type, IList<string> keys)
   => await _categorysRepository.ListAsync(new CategoriesSpecification(type, keys));
   public async Task<IEnumerable<Category>> FetchAllAsync()
      => await _categorysRepository.ListAsync(new CategoriesSpecification());

   public async Task<Category?> GetByIdAsync(int id)
      => await _categorysRepository.GetByIdAsync(id);

   public async Task<Category> CreateAsync(Category Category)
		=> await _categorysRepository.AddAsync(Category);

		public async Task UpdateAsync(Category Category)
		=> await _categorysRepository.UpdateAsync(Category);

}
