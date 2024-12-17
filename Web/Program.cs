using Serilog;
using Serilog.Events;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.DataAccess;
using Microsoft.AspNetCore.Identity;
using ApplicationCore.Models;
using ApplicationCore.Settings;
using ApplicationCore.Consts;
using ApplicationCore.DI;
using Web.Filters;
using System.Text.Json.Serialization;
using Infrastructure.Helpers;
using QuestPDF.Infrastructure;
using Infrastructure.Services;
using Microsoft.IdentityModel.Tokens;

Log.Logger = new LoggerConfiguration()
   .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
   .Enrich.FromLogContext()
   .WriteTo.Console()
   .CreateBootstrapLogger();

try
{
   Log.Information("Starting web application");
   var builder = WebApplication.CreateBuilder(args);
   var Configuration = builder.Configuration;
   builder.Host.UseSerilog((context, services, configuration) => configuration
         .ReadFrom.Configuration(context.Configuration)
         .ReadFrom.Services(services)
         .Enrich.FromLogContext());

   #region Autofac
   builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
   builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
   {
      builder.RegisterModule<ApplicationCoreModule>();
   });
   #endregion

   #region Add Configurations
   builder.Services.Configure<LdapSettings>(Configuration.GetSection(SettingsKeys.Ldap));
   builder.Services.Configure<AppSettings>(Configuration.GetSection(SettingsKeys.App));
   builder.Services.Configure<AdminSettings>(Configuration.GetSection(SettingsKeys.Admin));
   builder.Services.Configure<AttachmentSettings>(Configuration.GetSection(SettingsKeys.Attachment));
   builder.Services.Configure<AuthSettings>(Configuration.GetSection(SettingsKeys.Auth));
   builder.Services.Configure<CompanySettings>(Configuration.GetSection(SettingsKeys.Company));
   builder.Services.Configure<MailSettings>(Configuration.GetSection(SettingsKeys.Mail));
   builder.Services.Configure<JudSettings>(Configuration.GetSection(SettingsKeys.Judicial));
   builder.Services.Configure<Jud3Settings>(Configuration.GetSection(SettingsKeys.Jud3));
   //builder.Services.Configure<List<DbSettings>>(Configuration.GetSection("DbSettings"));
   #endregion

   string connectionString = Configuration.GetConnectionString("Default")!;
   bool usePostgreSql = Configuration[$"{SettingsKeys.Db}:Provider"].EqualTo(DbProvider.PostgreSql);
   if (usePostgreSql)
   {
      builder.Services.AddDbContext<DefaultContext>(options =>
                  options.UseNpgsql(connectionString));
   }
   else
   {
      builder.Services.AddDbContext<DefaultContext>(options =>
                  options.UseSqlServer(connectionString));
   }


   #region AddIdentity
   builder.Services.AddIdentity<User, Role>(options =>
   {
      options.User.RequireUniqueEmail = false;
   })
   .AddEntityFrameworkStores<DefaultContext>()
   .AddDefaultTokenProviders();
   #endregion

   builder.Services.AddOpenIddict()
    .AddServer(options =>
    {
       // Register the encryption credentials. This sample uses a symmetric
       // encryption key that is shared between the server and the API project.
       //
       // Note: in a real world application, this encryption key should be
       // stored in a safe place (e.g in Azure KeyVault, stored as a secret).
       options.AddEncryptionKey(new SymmetricSecurityKey(
           Convert.FromBase64String("DRjd/GnduI3Efzen9V9BvbNUfc/VKgXltV7Kbk9sMkY=")));

       // Register the signing credentials.
       options.AddDevelopmentSigningCertificate();
    });

   #region AddFilters
   builder.Services.AddScoped<DevelopmentOnlyFilter>();
   #endregion

   string key = Configuration[$"{SettingsKeys.App}:Key"]!;
   if (String.IsNullOrEmpty(key))
   {
      throw new Exception("app key not been set.");
   }

   builder.Services.AddScoped<ICryptoService>(provider => new AesCryptoService(key));

   builder.Services.AddCorsPolicy(Configuration);
   builder.Services.AddJwtBearer(Configuration);
   builder.Services.AddAuthorizationPolicy();

   builder.Services.AddDtoMapper();
   builder.Services.AddControllers()
   .AddJsonOptions(options =>
   {
      options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
   });


   builder.Services.AddSwagger(Configuration);

   QuestPDF.Settings.License = LicenseType.Community;

   var app = builder.Build();
   if (usePostgreSql)
   {
      AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
   }
   app.UseDefaultFiles();
   app.UseStaticFiles();

   app.UseSerilogRequestLogging();

   if (app.Environment.IsDevelopment())
   {
      if (Configuration[$"{SettingsKeys.Developing}:SeedDatabase"].ToBoolean())
      {
         // Seed Database
         using (var scope = app.Services.CreateScope())
         {
            var services = scope.ServiceProvider;
            try
            {
               await SeedData.EnsureSeedData(services, Configuration);
            }
            catch (Exception ex)
            {
               Log.Fatal(ex, "SeedData Error");
            }
         }
      }

      app.UseSwagger();
      app.UseSwaggerUI();
   }
   else
   {
      app.UseHttpsRedirection();
   }



   app.UseCors("Api");
   app.UseAuthentication();
   app.UseAuthorization();

   app.MapControllers();
   app.MapFallbackToFile("/index.html");
   app.Run();
}
catch (Exception ex)
{
   Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
   Log.Information("finally");
   Log.CloseAndFlush();
}





