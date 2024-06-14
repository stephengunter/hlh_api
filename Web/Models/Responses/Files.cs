namespace Web.Models;

public class FilesResponse
{
	public FilesResponse(List<FileViewModel> fileViewModels)
   {
      FileViewModels = fileViewModels;
   }

   public List<FileViewModel> FileViewModels { get; set; }
}

public class FileViewModel
{
   public string Name { get; set; }
   public string Path { get; set; }
}