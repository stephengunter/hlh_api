using ApplicationCore.Helpers;
using Infrastructure.Helpers;

namespace Web.Models;
public class OrdersRequest
{
	public string Key { get; set; } = String.Empty;
	public List<int> Ids { get; set; }
}