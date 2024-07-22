using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using Infrastructure.Entities;

namespace ApplicationCore.Services;

public interface IItemService
{
   Task<IEnumerable<Item>> FetchAsync();
   Task<IEnumerable<Item>> FetchAsync(string postType, int postId);
   Task<IEnumerable<Item>> FetchAsync(EntityBase entity);
   Task<Item?> GetByIdAsync(int id);

   Task<Item> CreateAsync(Item entity);
	Task UpdateAsync(Item entity);
}

public class ItemService : IItemService
{
	private readonly IDefaultRepository<Item> _itemRepository;

	public ItemService(IDefaultRepository<Item> itemRepository)
	{
      _itemRepository = itemRepository;
	}

   public async Task<IEnumerable<Item>> FetchAsync()
      => await _itemRepository.ListAsync(new ItemsSpecification());

   public async Task<IEnumerable<Item>> FetchAsync(EntityBase entity)
   {
      string type = entity.GetType().Name;
      return await _itemRepository.ListAsync(new ItemsSpecification(type, entity.Id));
   }

   public async Task<IEnumerable<Item>> FetchAsync(string postType, int postId)
      => await _itemRepository.ListAsync(new ItemsSpecification(postType, postId));


   public async Task<Item?> GetByIdAsync(int id)
      => await _itemRepository.GetByIdAsync(id);

   public async Task<Item> CreateAsync(Item entity)
		=> await _itemRepository.AddAsync(entity);

		public async Task UpdateAsync(Item entity)
		=> await _itemRepository.UpdateAsync(entity);

}
