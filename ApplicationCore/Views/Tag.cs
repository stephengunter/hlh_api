using Infrastructure.Views;

namespace ApplicationCore.Views;

public class TagViewModel : BaseRecordView
{
   public string Key { get; set; } = String.Empty;
   public string Title { get; set; } = String.Empty;
}

