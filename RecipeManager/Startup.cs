using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeManager.Services;

namespace RecipeManager
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            IConfigurationSection recipeDbConfig = _config.GetSection("RecipeDb");
            var recipeDbService = InitializeRecipeDbService(recipeDbConfig);
            services.AddSingleton<IRecipeDbService>(recipeDbService);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default",
                    "{controller=Recipe}/{action=Index}/{id?}");
            });
        }

        // Initialize Cosmos database client from appsettings and return a recipeDbService instance
        private static RecipeDbService InitializeRecipeDbService(IConfigurationSection recipeDbConfig)
        {
            string dbName = recipeDbConfig.GetSection("DatabaseName").Value;
            string containerName = recipeDbConfig.GetSection("ContainerName").Value;
            string account = recipeDbConfig.GetSection("Account").Value;
            string key = recipeDbConfig.GetSection("Key").Value;

            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(account, key);
            CosmosClient dbClient = clientBuilder.WithConnectionModeDirect().Build();
            RecipeDbService recipeDbService = new RecipeDbService(dbClient, dbName, containerName);
            return recipeDbService;
        }
    }
}
