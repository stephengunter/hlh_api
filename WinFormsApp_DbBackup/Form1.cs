using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using WinFormsApp_DbBackup.Components;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;


namespace WinFormsApp_DbBackup
{
   public partial class Form1 : Form
   {
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

      }

      protected override void OnFormClosing(FormClosingEventArgs e)
      {
         base.OnFormClosing(e);
      }
   }
}
