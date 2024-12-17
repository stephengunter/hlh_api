namespace WinFormCleanFiles;

partial class Form1
{
   private Label label1;
   private TextBox textBox1;
   private Label label2;
   private DateTimePicker dateTimePicker;
   private Button button;
   private Label labelFeedback;
   /// <summary>
   ///  Required designer variable.
   /// </summary>
   private System.ComponentModel.IContainer components = null;

   /// <summary>
   ///  Clean up any resources being used.
   /// </summary>
   /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
   protected override void Dispose(bool disposing)
   {
      if (disposing && (components != null))
      {
         components.Dispose();
      }
      base.Dispose(disposing);
   }

   #region Windows Form Designer generated code

   /// <summary>
   ///  Required method for Designer support - do not modify
   ///  the contents of this method with the code editor.
   /// </summary>
   private void InitializeComponent()
   {
      // Label 1
      label1 = new Label
      {
         Text = "Select Folder:",
         Location = new System.Drawing.Point(20, 20),
         Width = 100
      };

      // TextBox 1
      textBox1 = new TextBox
      {
         Location = new System.Drawing.Point(130, 20),
         Width = 300,
         ReadOnly = true // Prevent manual input
      };
      // Add event handler to open folder browser dialog
      textBox1.Click += TextBox1_Click;
      // Label 2
      label2 = new Label
      {
         Text = "Select Date:",
         Location = new System.Drawing.Point(20, 60),
         Width = 100
      };
      // DateTimePicker
      dateTimePicker = new DateTimePicker
      {
         Location = new System.Drawing.Point(130, 60),
         Width = 150,
         Format = DateTimePickerFormat.Short,
         Value = DateTime.Now.AddYears(-1) // Set initial date to one year from now
      };

      // Button
      button = new Button
      {
         Text = "Submit",
         Location = new System.Drawing.Point(20, 100),
         Width = 260
      };
      button.Click += Button_Click;

      // Label: Feedback
      labelFeedback = new Label
      {
         Location = new System.Drawing.Point(12, 130),
         Size = new System.Drawing.Size(300, 50),
         Text = "",
         AutoSize = true
      };

      // Add controls to the form
      Controls.Add(label1);
      Controls.Add(textBox1);
      Controls.Add(label2); 
      Controls.Add(dateTimePicker);
      Controls.Add(this.labelFeedback);
      Controls.Add(button);

      // Set form properties
      Text = "清除過時檔案";
      Size = new System.Drawing.Size(480, 240);
      StartPosition = FormStartPosition.CenterScreen;
   }

   #endregion
}
