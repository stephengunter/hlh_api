using System;
using System.IO;
using System.Text;

public class SqlScriptGenerator
{
   public static string GenerateRestoreScript(string databaseName, string fullBackupPath, string diffBackupPath)
   {
      // Start building the SQL script using StringBuilder
      StringBuilder sqlScript = new StringBuilder();

      // Set the database to single-user mode with ROLLBACK IMMEDIATE
      sqlScript.AppendLine("USE master;");
      sqlScript.AppendLine();
      sqlScript.AppendLine($"-- Step 1: Set the database {databaseName} to single-user mode with ROLLBACK IMMEDIATE");
      sqlScript.AppendLine($"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;");
      sqlScript.AppendLine();

      // Restore the full backup with NORECOVERY to prepare for differential restore
      sqlScript.AppendLine($"-- Step 2: Restore the full backup with NORECOVERY");
      sqlScript.AppendLine($"RESTORE DATABASE [{databaseName}]");
      sqlScript.AppendLine($"FROM DISK = '{fullBackupPath}'");
      sqlScript.AppendLine("WITH NORECOVERY, REPLACE;");
      sqlScript.AppendLine();

      // Restore the differential backup with RECOVERY
      sqlScript.AppendLine($"-- Step 3: Restore the differential backup with RECOVERY");
      sqlScript.AppendLine($"RESTORE DATABASE [{databaseName}]");
      sqlScript.AppendLine($"FROM DISK = '{diffBackupPath}'");
      sqlScript.AppendLine("WITH RECOVERY, REPLACE;");
      sqlScript.AppendLine();

      // Set the database back to multi-user mode
      sqlScript.AppendLine($"-- Step 4: Set the database {databaseName} back to multi-user mode");
      sqlScript.AppendLine($"ALTER DATABASE [{databaseName}] SET MULTI_USER;");

      return sqlScript.ToString();
   }

   // Method to save the generated SQL script to a .sql file
   public static void SaveSqlScriptToFile(string sqlScript, string filePath)
   {
      // Write the SQL script to the specified file
      File.WriteAllText(filePath, sqlScript);
      Console.WriteLine($"SQL script has been written to {filePath}");
   }
}
