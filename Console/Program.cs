using Humanizer;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace IKVue
{
  
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
         //AddMutations(Path.Combine(src, "store"), name);
         //AddService(Path.Combine(src, "services"), name);
         //AddModule(Path.Combine(src, "store"), name);
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
      
      static void AddMutations(string storePath, string name)
      {
         string path = Path.Combine(storePath, "mutations.type.js");
         string result = "";
         string[] lines = File.ReadAllLines(path);
         for (int i = 0; i < lines.Length; i++)
         {
            if (lines[i].Trim() == "//NEW")
            {
               string newLine = lines[i] + Environment.NewLine;
               newLine += $"export const SET_{name.Pluralize().ToUpper()} = 'set{name.Pluralize().Titleize()}'";
               newLine += Environment.NewLine;


               result += newLine;
            }
            else result += lines[i];

            result += Environment.NewLine;
         }

         File.WriteAllText(path, result);
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

      static void AddModule(string storePath, string name)
      {
         string modulesPath = Path.Combine(storePath, "modules");
         string template = "article";
         string result = "";
         using (var reader = File.OpenText(Path.Combine(modulesPath, $"{template.Pluralize()}.module.js")))
         {
            result = reader.ReadToEnd();
         }

         result = result.Replace(template, name) //article, name
                        .Replace(template.Titleize(), name.Titleize())  //Article, Name
                        .Replace(template.Pluralize(), name.Pluralize())  //articles, names
                        .Replace(template.Pluralize().Titleize(), name.Pluralize().Titleize())  //Articles, Names
                        .Replace(template.ToUpper(), name.ToUpper()) //ARTICLE, NAME         
                        .Replace(template.Pluralize().ToUpper(), name.Pluralize().ToUpper()); //ARTICLES, NAMES

         string path = Path.Combine(modulesPath, $"{name.Pluralize()}.module.js");
         File.WriteAllText(path, result);

         result = "";
         path = Path.Combine(storePath, "index.js");
         string[] lines = File.ReadAllLines(path);

         for (int i = 0; i < lines.Length; i++)
         {
            string newLine = lines[i];
            if (!string.IsNullOrEmpty(newLine))
            {
               string[] words = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
               if (words.Length > 2 && words[0] == "import" && words[1] == "articles")
               {
                  newLine += Environment.NewLine;
                  newLine += $"import {name.Pluralize()} from './modules/{name.Pluralize()}.module'";
               }
               else if (words.Length == 1 && words[0] == "articles,")
               {
                  newLine += Environment.NewLine;
                  newLine += newLine.Replace("articles,", $"{name.Pluralize()},");
               }
            }
            result += newLine;
            result += Environment.NewLine;
         }
         File.WriteAllText(path, result);

      }

      static void AddService(string servicePath, string name)
      {
         string template = "article";
         string result = "";
         using (var reader = File.OpenText(Path.Combine(servicePath, $"{template.Pluralize()}.service.js")))
         {
            result = reader.ReadToEnd();
         }

         result = result.Replace(template, name);

         string path = Path.Combine(servicePath, $"{name.Pluralize()}.service.js");
         File.WriteAllText(path, result);
      }

   }
}