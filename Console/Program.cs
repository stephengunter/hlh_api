using Humanizer;
using System;
using System.CommandLine;

namespace ConsoleDev;


class Program
{
   static string TEMPLATE_NAME = "article";

   static int Main(string[] args)
   {
      var rootCommand = new RootCommand("");
      var nameOption = new Option<string>(
        name: "--name",
        description: "name of model"
      );
      var readCommand = new Command("add", "add model") { nameOption };

      rootCommand.AddCommand(readCommand);
      readCommand.SetHandler((name) =>
      {
         AddFile(name);
      },
         nameOption
      );

      return rootCommand.InvokeAsync(args).Result;
   }

   internal static void AddFile(string name)
   {
      return;
      var root = Directory.GetParent(Directory.GetCurrentDirectory());

      var applicationCore = Path.Combine(root!.FullName, "ApplicationCore");
      if (!Directory.Exists(applicationCore)) throw new Exception("ApplicationCore Not Found.");

      var web = Path.Combine(root.FullName, "Web");
      if (!Directory.Exists(applicationCore)) throw new Exception("Web Not Found.");

      var templates = Path.Combine(Directory.GetCurrentDirectory(), "templates");
      if (!Directory.Exists(templates)) throw new Exception("templates Not Found.");

      name = name.ToLower();
      AddModel(templates, name, applicationCore);
      AddViewModel(templates, name, applicationCore);
      AddDtoMapper(templates, name, applicationCore);
      AddSpecifications(templates, name, applicationCore);
      AddServices(templates, name, applicationCore);
      AddHelpers(templates, name, applicationCore);

      AddControllers(templates, name, web);
   }

   static void AddModel(string templatesPath, string name, string targetPath)
   {
      string folder = "Models";
      string path = Path.Combine(templatesPath, folder, $"{TEMPLATE_NAME.Titleize()}.txt");
      string result = ReplaceTemplate(File.ReadAllText(path), name);

      string fileName = $"{name.Titleize()}.cs";
      path = Path.Combine(targetPath, folder, fileName);
      File.WriteAllText(path, result);

      Console.WriteLine($"Model {fileName} Added");
   }
   static void AddViewModel(string templatesPath, string name, string targetPath)
   {
      string folder = "Views";
      string path = Path.Combine(templatesPath, folder, $"{TEMPLATE_NAME.Titleize()}.txt");
      string result = ReplaceTemplate(File.ReadAllText(path), name);

      string fileName = $"{name.Titleize()}.cs";
      path = Path.Combine(targetPath, folder, fileName);
      File.WriteAllText(path, result);

      Console.WriteLine($"ViewModel {fileName} Added");
   }
   static void AddDtoMapper(string templatesPath, string name, string targetPath)
   {
      string folder = "DtoMapper";
      string path = Path.Combine(templatesPath, folder, $"{TEMPLATE_NAME.Titleize()}.txt");
      string result = ReplaceTemplate(File.ReadAllText(path), name);

      string fileName = $"{name.Titleize()}.cs";
      path = Path.Combine(targetPath, folder, fileName);
      File.WriteAllText(path, result);

      Console.WriteLine($"DtoMapper {fileName} Added");
   }
   static void AddSpecifications(string templatesPath, string name, string targetPath)
   {
      string folder = "Specifications";
      string path = Path.Combine(templatesPath, folder, $"{TEMPLATE_NAME.Pluralize().Titleize()}.txt");
      string result = ReplaceTemplate(File.ReadAllText(path), name);

      string fileName = $"{name.Pluralize().Titleize()}.cs";
      path = Path.Combine(targetPath, folder, fileName);
      File.WriteAllText(path, result);

      Console.WriteLine($"Specifications {fileName} Added");
   }
   static void AddServices(string templatesPath, string name, string targetPath)
   {
      string folder = "Services";
      string path = Path.Combine(templatesPath, folder, $"{TEMPLATE_NAME.Pluralize().Titleize()}.txt");
      string result = ReplaceTemplate(File.ReadAllText(path), name);

      string fileName = $"{name.Pluralize().Titleize()}.cs";
      path = Path.Combine(targetPath, folder, fileName);
      File.WriteAllText(path, result);

      Console.WriteLine($"Services {fileName} Added");
   }
   static void AddHelpers(string templatesPath, string name, string targetPath)
   {
      string folder = "Helpers/Models";
      string path = Path.Combine(templatesPath, folder, $"{TEMPLATE_NAME.Pluralize().Titleize()}.txt");
      string result = ReplaceTemplate(File.ReadAllText(path), name);

      string fileName = $"{name.Pluralize().Titleize()}.cs";
      path = Path.Combine(targetPath, folder, fileName);
      File.WriteAllText(path, result);

      Console.WriteLine($"Helpers {fileName} Added");
   }
   static void AddControllers(string templatesPath, string name, string targetPath)
   {
      string folder = "Controllers/Admin";
      string path = Path.Combine(templatesPath, folder, $"{TEMPLATE_NAME.Pluralize().Titleize()}Controller.txt");
      string result = ReplaceTemplate(File.ReadAllText(path), name);

      string fileName = $"{name.Pluralize().Titleize()}Controller.cs";
      path = Path.Combine(targetPath, folder, fileName);
      File.WriteAllText(path, result);

      Console.WriteLine($"Controller {fileName} Added");
   }

   static string ReplaceTemplate(string content, string name)
   {
      return content.Replace(TEMPLATE_NAME, name) //article, name
                        .Replace(TEMPLATE_NAME.Titleize(), name.Titleize())  //Article, Name
                        .Replace(TEMPLATE_NAME.Pluralize(), name.Pluralize())  //articles, names
                        .Replace(TEMPLATE_NAME.Pluralize().Titleize(), name.Pluralize().Titleize())  //Articles, Names
                        .Replace(TEMPLATE_NAME.ToUpper(), name.ToUpper()) //ARTICLE, NAME         
                        .Replace(TEMPLATE_NAME.Pluralize().ToUpper(), name.Pluralize().ToUpper()); //ARTICLES, NAMES
   }

}