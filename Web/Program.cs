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
using ApplicationCore.Helpers;
using ApplicationCore.DI;
using Web.Filters;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Builder;
using ApplicationCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Infrastructure.Helpers;
using Autofac.Core;
using Google;

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
   builder.Services.Configure<DbSettings>(Configuration.GetSection(SettingsKeys.Db));
   builder.Services.Configure<AppSettings>(Configuration.GetSection(SettingsKeys.App));
	builder.Services.Configure<AdminSettings>(Configuration.GetSection(SettingsKeys.Admin));
	builder.Services.Configure<AuthSettings>(Configuration.GetSection(SettingsKeys.Auth));
	builder.Services.Configure<MailSettings>(Configuration.GetSection(SettingsKeys.Mail));
   builder.Services.Configure<JudSettings>(Configuration.GetSection(SettingsKeys.Jud));
   #endregion

   string connectionString = Configuration.GetConnectionString("Default")!;
   if (Configuration[$"{SettingsKeys.Db}:Provider"].EqualTo(DbProvider.PostgreSql))
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
		options.User.RequireUniqueEmail = true;
	})
	.AddEntityFrameworkStores<DefaultContext>()
   .AddDefaultTokenProviders();
   #endregion

   #region AddFilters
   builder.Services.AddScoped<DevelopmentOnlyFilter>();
   #endregion

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

	var app = builder.Build();
	AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

	app.UseSerilogRequestLogging(); 

	if (app.Environment.IsDevelopment())
	{
		if(Configuration[$"{SettingsKeys.Developing}:SeedDatabase"].ToBoolean())
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
   //app.UseStaticFiles();
   //app.UseRouting();

   
   app.UseCors();
   app.UseAuthentication();
   app.UseAuthorization();

	app.MapControllers();
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






