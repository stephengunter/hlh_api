namespace Infrastructure.Entities;

public interface IReviewable
{
	bool Reviewed { get; set; }
   string? ReviewedBy { get; set; }
}
