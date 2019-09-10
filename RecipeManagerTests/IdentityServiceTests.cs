using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.DocumentDB;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using RecipeManager.Models;
using RecipeManager.Services;

namespace RecipeManager.Tests
{
    [TestFixture]
    public class IdentityServiceIntegrationTests
    {
        private string _dbName = "RecipeDbTest";
        private DocumentClient _dbClient;
        private UserManager<RecipeUser> _manager;

        [OneTimeSetUp]
        public void ClassInit()
        {
            string containerName = "RecipeTests";
            string account = "https://localhost:8081";
            string key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

            _dbClient = new DocumentClient(new Uri(account), key);
            _dbClient.CreateDatabaseIfNotExistsAsync(new Database { Id = _dbName }).GetAwaiter().GetResult();
            DocumentCollection collectionDefinition = new DocumentCollection
            {
                Id = containerName
            };
            var userCollection = _dbClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(_dbName), collectionDefinition).GetAwaiter().GetResult();

            var services = new ServiceCollection();
            services
                .AddIdentity<RecipeUser, IdentityRole>()
                .RegisterDocumentDBStores<RecipeUser, IdentityRole>(_dbClient,
                    (p) => userCollection);
            services.AddLogging();
            var provider = services.BuildServiceProvider();
            _manager = provider.GetRequiredService<UserManager<RecipeUser>>();
        }

        [OneTimeTearDown]
        public void ClassCleanup()
        {
            var dbUri = UriFactory.CreateDatabaseUri(_dbName);
            _dbClient.DeleteDatabaseAsync(dbUri).GetAwaiter().GetResult();
            _dbClient.Dispose();
        }

        [Test]
        public async Task Should_Be_Able_To_Create_User_With_Valid_UserName_and_Password()
        {
            var testUser = new RecipeUser
            {
                UserName = "test@username.com"
            };
            var result = await _manager.CreateAsync(testUser, "P@ssw0rd1!");
            Assert.That(result.Succeeded, Is.True);
            var user = await _manager.FindByNameAsync(testUser.UserName);
            Assert.That(user.UserName, Is.EqualTo(testUser.UserName));
        }


    }
}
