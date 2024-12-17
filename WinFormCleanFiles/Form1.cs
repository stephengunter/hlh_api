using Microsoft.VisualBasic.FileIO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WinFormCleanFiles;

public partial class Form1 : Form
{
   public Form1()
   {
      InitializeComponent();
   }
   private void TextBox1_Click(object sender, EventArgs e)
   {
      using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
      {
         folderBrowserDialog.Description = "Select a folder";
         if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
         {
            textBox1.Text = folderBrowserDialog.SelectedPath;
         }
      }
   }
   private void Button_Click(object sender, EventArgs e)
   {
      labelFeedback.Text = "";
      string folderPath = textBox1.Text;
      var selectedDate = dateTimePicker.Value;

      if (string.IsNullOrEmpty(folderPath))
      {
         MessageBox.Show("Please select a folder.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
         return;
      }

      DialogResult confirm = MessageBox.Show(
          $"此程式將把資料夾: {folderPath}\n 中，{selectedDate.ToShortDateString()} 之前的檔案都刪除，是否確定執行?",
          "是否確定刪除檔案",
          MessageBoxButtons.YesNo,
          MessageBoxIcon.Question
      );

      if (confirm == DialogResult.Yes)
      {
         try
         {
            button.Enabled = false;
            var result = RemoveFiles(folderPath, selectedDate);
            labelFeedback.Text = $"Files removed: {result.FilesRemoved}, Total size: {result.TotalSize} bytes";
            button.Enabled = true;
         }
         catch (Exception ex)
         {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
        
      }
   }
   private (int FilesRemoved, long TotalSize) RemoveFiles(string folderPath, DateTime date)
   {
      if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
      {
         throw new Exception("Invalid folder path.");
      }

      int filesRemoved = 0;
      long totalSize = 0;

      // Get all files in the specified folder
      var files = Directory.GetFiles(folderPath, "*.*", System.IO.SearchOption.AllDirectories);
      foreach (var file in files)
      {
         // Get the creation time of the file
         DateTime creationTime = File.GetLastWriteTime(file);

         // Check if the file was created before the specified date
         if (creationTime < date)
         {
            // Get the size of the file
            long fileSize = new FileInfo(file).Length;

            // Move the file to Recycle Bin
            FileSystem.DeleteFile(
                file,
                UIOption.OnlyErrorDialogs, // No UI unless errors occur
                RecycleOption.SendToRecycleBin // Send to Recycle Bin
            );

            // Update counters
            filesRemoved++;
            totalSize += fileSize;
         }
      }

      return (filesRemoved, totalSize);
   }
}
