using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using NUnit.Framework;
using RecipeManager.Models;
using RecipeManager.Services;

namespace RecipeManager.Tests
{
    [TestFixture]
    public class RecipeDbServiceIntegrationTests
    {
        private string _dbName = "RecipeDbTest";
        private CosmosClient _dbClient;
        private RecipeDbService _recipeTestService;
        private List<Recipe> _sampleData;

        [OneTimeSetUp]
        public void ClassInit()
        {
            string containerName = "RecipeTests";
            string account = "https://localhost:8081";
            string key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

            CosmosClientBuilder clientBuilder = new CosmosClientBuilder(account, key);
            _dbClient = clientBuilder.WithConnectionModeDirect().Build();
            _recipeTestService = new RecipeDbService(_dbClient, _dbName, containerName);

            _sampleData = new List<Recipe>();
            _sampleData.Add(
                new Recipe
                {
                    Id = "7288cb9e-fd10-4843-be36-b2a734216c1b",
                    Title = "Test Recipe 1",
                    Description = "This is a test description for test recipe 1.",
                    RecipeSteps = new List<string> { "Recipe 1 step 1", "Recipe 1 step 2" },
                    Notes = "Recipe 1 notes.",
                    UserId = "ced4bc56-ecd4-4d47-81bb-e74c9406f282"
                });
            _sampleData.Add(
                new Recipe
                {
                    Id = "658e9a3b-524c-4b6d-a7ce-23ef521d7e3d",
                    Title = "Test Recipe 2",
                    Description = "This is a test description for test recipe 2 which has no notes.",
                    RecipeSteps = new List<string> {"Recipe 2 step 1", "Recipe 2 step 2"},
                    UserId = "2152b7e8-9ce1-4b70-854f-b0463bbf640a"
                });
        }

        [OneTimeTearDown]
        public void ClassCleanup()
        {
            var response = _dbClient.GetDatabase(_dbName);
            response.DeleteAsync().GetAwaiter().GetResult();
            _dbClient.Dispose();
        }

        [Test]
        public async Task RecipeDbService_GetRecipeAsync_Should_Give_Same_Data_As_Passed_To_AddRecipeAsync()
        {
            await _recipeTestService.AddRecipeAsync(_sampleData[0]);
            var result = await _recipeTestService.GetRecipeAsync(_sampleData[0].Id);
            Assert.That(result.Id,Is.EqualTo(_sampleData[0].Id));
            Assert.That(result.Description,Is.EqualTo(_sampleData[0].Description));
            Assert.That(result.Notes,Is.EqualTo(_sampleData[0].Notes));
            Assert.That(result.RecipeSteps,Is.EqualTo(_sampleData[0].RecipeSteps));
            Assert.That(result.Title,Is.EqualTo(_sampleData[0].Title));
            Assert.That(result.UserId,Is.EqualTo(_sampleData[0].UserId));
        }
    }
}