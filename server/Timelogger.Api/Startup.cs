using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Timelogger.Entities;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace Timelogger.Api
{
	public class Startup
	{
		private readonly IWebHostEnvironment _environment;
		public IConfigurationRoot Configuration { get; }

		public Startup(IWebHostEnvironment env)
		{
			_environment = env;

			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			// Add framework services.
			services.AddControllers();
			services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("e-conomic interview"),ServiceLifetime.Singleton);
			//services.AddSingleton<ApiContext>(s=> new ApiContext(new DbContextOptions<ApiContext>()));
			services.AddMemoryCache();
			services.AddLogging(builder =>
			{
				builder.AddConsole();
				builder.AddDebug();
			});
			services.AddSwaggerGen();

			if (_environment.IsDevelopment())
			{
				services.AddCors();
			}
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseCors(builder => builder
					.AllowAnyMethod()
					.AllowAnyHeader()
					.SetIsOriginAllowed(origin => true)
					.AllowCredentials());
			}
			app.UseHttpsRedirection();
			
			app.UseRouting();

			app.UseAuthorization();

			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			});
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
			string projectsDataFilePath = Configuration.GetValue<string>("ProjectsDataFilePath");
			var serviceScopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                SeedDatabase(scope, projectsDataFilePath);
            }
        }

		private static void SeedDatabase(IServiceScope scope, string filePath)
		{

			var context = scope.ServiceProvider.GetService<ApiContext>();
            var json = File.ReadAllText($"../{filePath}");
            var list = JsonSerializer.Deserialize<List<Project>>(json);
            list.ForEach(p => context.Projects.Add(p));
            
			context.SaveChanges();
		}
	}
}