using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RecipeManager.Models;
using RecipeManager.Services;
using IdentityRole = Microsoft.AspNetCore.Identity.DocumentDB.IdentityRole;

namespace RecipeManager
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private string _containerName;
        private ResourceResponse<DocumentCollection> _collection;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            IConfigurationSection recipeDbConfig = _config.GetSection("RecipeDb");
            var recipeDbService = InitializeRecipeDbService(recipeDbConfig, out DocumentClient dbClient);
            services.AddSingleton<IRecipeDbService>(recipeDbService);

            services.AddIdentity<RecipeUser, IdentityRole>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .RegisterDocumentDBStores<RecipeUser, IdentityRole>(dbClient,
                    (p) => _collection)
                .AddDefaultTokenProviders();
            services.AddLogging();

// TODO: Remove after adding registration support
#if DEBUG
            var provider = services.BuildServiceProvider();
            var userManager = provider.GetRequiredService<UserManager<RecipeUser>>();

            RecipeUser firstUser = new RecipeUser { UserName = "user@test.com"};
            var user = userManager.FindByNameAsync(firstUser.UserName).GetAwaiter().GetResult();
            if (user == null)
            {
                var result = userManager.CreateAsync(firstUser, "P@ssw0rd!").GetAwaiter().GetResult();
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException("User not created");
                }
            }
#endif           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseNodeModules(env);
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default",
                    "{controller=Recipe}/{action=Index}/{id?}");
            });
        }

        // Initialize Cosmos database client from appsettings and return a recipeDbService instance
        private RecipeDbService InitializeRecipeDbService(IConfigurationSection recipeDbConfig, out DocumentClient dbClient)
        {
            string dbName = recipeDbConfig.GetSection("DatabaseName").Value;
            _containerName = recipeDbConfig.GetSection("ContainerName").Value;
            string account = recipeDbConfig.GetSection("Account").Value;
            string key = recipeDbConfig.GetSection("Key").Value;

            dbClient = new DocumentClient(new Uri(account), key);
            dbClient.CreateDatabaseIfNotExistsAsync(new Database { Id = dbName }).GetAwaiter().GetResult();
            DocumentCollection collectionDefinition = new DocumentCollection
            {
                Id = _containerName
            };
            _collection = dbClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(dbName), collectionDefinition).GetAwaiter().GetResult();
            RecipeDbService recipeDbService = new RecipeDbService(dbClient, dbName, _containerName);
            return recipeDbService;
        }
    }
}
