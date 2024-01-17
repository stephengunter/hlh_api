using Infrastructure.Entities;

namespace ApplicationCore.Models;
public enum PostType
{
	Article = 0,
	None = -1
}

public class UploadFile : BaseUploadFile
{
	public PostType PostType { get; set; } = PostType.None;
	public int PostId { get; set; }

}
