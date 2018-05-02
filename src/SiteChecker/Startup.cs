using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SiteChecker.Configuration;
using SiteChecker.Infrastructure;
using SiteChecker.Mappers;
using SiteChecker.Models;
using SiteChecker.Models.ManageViewModels;
using SiteChecker.Repositories;
using SiteChecker.Services;
using SiteChecker.Tasks;

namespace SiteChecker
{
	public class Startup
	{
		public IConfiguration Configuration { get; }

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddTransient<IUserStore<ApplicationUser>, UserStore>();
			services.AddTransient<IRoleStore<ApplicationRole>, RoleStore>();
			
			services.AddIdentity<ApplicationUser, ApplicationRole>()
				.AddDefaultTokenProviders();

			services.AddScoped<IMapper<Sites, SiteEditViewModel>, SiteModelToSiteEditViewModelMapper>();
			services.AddScoped<IMapper<SiteEditViewModel, Sites>, SiteEditViewModelToSiteModelMapper>();
			services.AddScoped<IMapper<Sites, SiteViewModel>, SiteModelToSiteViewModelMapper>();

			services.AddMvc();
			
			services.Configure<SitesBackgroundSettings>(Configuration);

			services.AddScoped<IDatabaseConnectionFactory, SitesDatabaseConnectionFactory>();
			services.AddScoped<ISitesRepository, SitesRepository>();

			services.AddSingleton<ISiteChecker, Services.SiteChecker>();
			// Register Hosted Services
			services.AddSingleton<IHostedService, SitesAvailabilityService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseBrowserLink();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();
			app.UseAuthentication();

			app.UseMvcWithDefaultRoute();
		}
	}

	// Copyright (c) .NET Foundation. Licensed under the Apache License, Version 2.0. 
}