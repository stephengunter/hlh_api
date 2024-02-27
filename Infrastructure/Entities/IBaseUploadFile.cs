﻿namespace Infrastructure.Entities;

public interface IBaseUploadFile
{
	string? Path { get; set; }

	string? PreviewPath { get; set; }

	int Width { get; set; }

	int Height { get; set; }

	string? Type { get; set; }

	string? Name { get; set; }

	string? Title { get; set; }

	string? Description { get; set; }
}
