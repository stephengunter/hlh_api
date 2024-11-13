using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using WinFormsApp_DbBackup.Components;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;


namespace WinFormsApp_DbBackup
{
   public partial class Form1 : Form
   {
      private readonly System.Windows.Forms.Timer timer;
      private const string TEMP_FOLDER_PATH = @"C:\temp";
      private readonly WinFormsApp_DbBackup.App _app;
      public Form1(IServiceProvider services)
      {
         InitializeComponent();

         _app = services.GetRequiredService<WinFormsApp_DbBackup.App>();

         // Configure BlazorWebView to use the shared services
         blazorWebView1.HostPage = "wwwroot\\index.html";
         blazorWebView1.Services = services;

         // Add the root component you want to display in the Blazor WebView
         blazorWebView1.RootComponents.Add<WinFormsApp_DbBackup.Components.App>("#app");

         timer = new System.Windows.Forms.Timer();
         timer.Interval = 1000; // Set interval to 1000ms (1 second)
         timer.Tick += Timer_Tick; // Assign the event handler
         timer.Start(); // Start the timer

      }
      private void Timer_Tick(object sender, EventArgs e)
      {
         // Perform the action you want to execute periodically
         Console.WriteLine("Timer ticked at: " + DateTime.Now);

         // You can also call a method to update the Blazor component if needed
         // For example, you could update the component state with a timestamp
      }

      protected override void OnFormClosing(FormClosingEventArgs e)
      {
         // Stop and dispose the timer when the form is closing
         timer.Stop();
         timer.Dispose();
         base.OnFormClosing(e);
      }
   }
}
