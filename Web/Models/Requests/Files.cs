using ApplicationCore.Helpers;
using Infrastructure.Helpers;

namespace Web.Models;

public class FilesRequest
{
	public List<IFormFile> Files { get; set; } = new List<IFormFile>();

	public IFormFile? GetFile(string name)
	{
		if (Files.IsNullOrEmpty()) return null;
		return Files.FirstOrDefault(item => Path.GetFileNameWithoutExtension(item.FileName) == name);

	}
}