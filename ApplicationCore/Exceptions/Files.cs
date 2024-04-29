using Infrastructure.Entities;

namespace ApplicationCore.Exceptions;
public class FileNotExistException : Exception
{
	public FileNotExistException(string path) : base($"FileNotFound. Path: {path}")
	{

	}

   public FileNotExistException(EntityBase entity, string path) : base($"FileNotFound. Type: {entity.GetType().Name}  Id: {entity.Id}  Path: {path}")
   {

   }
}

public class MoveFileFailedException : Exception
{
   public MoveFileFailedException(EntityBase entity, string source, string dest) : base($"MoveFileFailedException. Type: {entity.GetType().Name}  Id: {entity.Id}  SourcePath: {source}  DestPath: {dest}")
   {

   }
}
