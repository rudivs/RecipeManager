using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            await _recipeTestService.DeleteRecipeAsync(_sampleData[0].Id);
        }

        [Test]
        public async Task
            RecipeDbService_AddRecipeAsync_Should_Throw_CosmosException_With_Status_Code_Conflict_When_Passed_Duplicate_Recipe()
        {
            await _recipeTestService.AddRecipeAsync(_sampleData[0]);
            var ex = Assert.ThrowsAsync<CosmosException>(async () => await _recipeTestService.AddRecipeAsync(_sampleData[0]));
            Assert.That(ex.StatusCode,Is.EqualTo(HttpStatusCode.Conflict));
            await _recipeTestService.DeleteRecipeAsync(_sampleData[0].Id);
        }

        [Test]
        public async Task
            RecipeDbService_AddRecipeAsync_Should_Throw_CosmosException_With_Status_Code_Conflict_When_Passed_Recipe_With_Existing_Id()
        {
            var testRecipe = new Recipe
            {
                Id = "7288cb9e-fd10-4843-be36-b2a734216c1b",
                Title = "Conflicting Recipe",
                Description = "Conflicting recipe description.",
                RecipeSteps = new List<string> { "step 1", "step 2" },
                Notes = "Conflicting recipe notes.",
                UserId = "ced4bc56-ecd4-4d47-81bb-e74c9406f282"
            };
            await _recipeTestService.AddRecipeAsync(_sampleData[0]);
            var ex = Assert.ThrowsAsync<CosmosException>(async () => await _recipeTestService.AddRecipeAsync(testRecipe));
            Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.Conflict));
            await _recipeTestService.DeleteRecipeAsync(_sampleData[0].Id);
        }

        [Test]
        public async Task RecipeDbService_AddRecipeAsync_Should_Throw_ArgumentException_If_Id_Not_Provided()
        {
            var testRecipe = new Recipe
            {
                Title = "Invalid Recipe",
                Description = "Invalid recipe description.",
                RecipeSteps = new List<string> { "step 1", "step 2" },
                Notes = "Invalid recipe notes.",
                UserId = "ced4bc56-ecd4-4d47-81bb-e74c9406f282"
            };
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _recipeTestService.AddRecipeAsync(testRecipe));
            Assert.That(ex.Message, Is.EqualTo("Please ensure that the recipe has a valid id."));
        }

        [Test]
        public async Task RecipeDbService_AddRecipeAsync_Should_Throw_ArgumentException_If_Title_Not_Provided()
        {
            var testRecipe = new Recipe
            {
                Id = "dc5162a5-9a27-4b52-81e8-ae2abff33bbb",
                Description = "Invalid recipe description.",
                RecipeSteps = new List<string> { "step 1", "step 2" },
                Notes = "Invalid recipe notes.",
                UserId = "ced4bc56-ecd4-4d47-81bb-e74c9406f282"
            };
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _recipeTestService.AddRecipeAsync(testRecipe));
            Assert.That(ex.Message, Is.EqualTo("Please ensure that the recipe has a title."));
        }

        [Test]
        public async Task RecipeDbService_AddRecipeAsync_Should_Throw_ArgumentException_If_Steps_Not_Provided()
        {
            var testRecipe = new Recipe
            {
                Id = "dc5162a5-9a27-4b52-81e8-ae2abff33bbb",
                Title = "Invalid Recipe",
                Description = "Invalid recipe description.",
                RecipeSteps = new List<string>( ),
                Notes = "Invalid recipe notes.",
                UserId = "ced4bc56-ecd4-4d47-81bb-e74c9406f282"
            };
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _recipeTestService.AddRecipeAsync(testRecipe));
            Assert.That(ex.Message, Is.EqualTo("Please ensure that the recipe has at least one step."));
        }

        [Test]
        public async Task RecipeDbService_GetRecipeAsync_Should_Return_Null_If_Not_In_Database()
        {
            var result = await _recipeTestService.GetRecipeAsync("does_not_exist");
            Assert.That(result,Is.Null);
        }


        [Test]
        public async Task RecipeDbService_DeleteRecipeAsync_Should_Work_When_Passed_Valid_Id()
        {
            await _recipeTestService.AddRecipeAsync(_sampleData[0]);
            await _recipeTestService.DeleteRecipeAsync(_sampleData[0].Id);
            var result = await _recipeTestService.GetRecipeAsync(_sampleData[0].Id);
            Assert.That(result,Is.Null);
        }

        [Test]
        public async Task RecipeDbService_DeleteRecipeAsync_Should_Throw_CosmosException_With_Status_Code_NotFound_When_Passed_Invalid_Id()
        {
            var ex = Assert.ThrowsAsync<CosmosException>(async () =>
                await _recipeTestService.DeleteRecipeAsync("does_not_exist"));
            Assert.That(ex.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task RecipeDbService_GetRecipesAsync_Should_Return_Empty_List_If_There_Are_No_Recipes_In_Database()
        {
            var result = await _recipeTestService.GetRecipesAsync();
            Assert.That(result.Any(),Is.False);
        }

        [Test]
        public async Task RecipeDbService_GetRecipesAsync_Should_Return_All_Recipes_In_Database_In_Correct_Order()
        {
            await _recipeTestService.AddRecipeAsync(_sampleData[0]);
            await _recipeTestService.AddRecipeAsync(_sampleData[1]);
            var result = await _recipeTestService.GetRecipesAsync();
            Assert.That(result.Count(),Is.EqualTo(2));
            Assert.That(result[0].Id,Is.EqualTo(_sampleData[0].Id));
            Assert.That(result[1].Id,Is.EqualTo(_sampleData[1].Id));
            await _recipeTestService.DeleteRecipeAsync(_sampleData[0].Id);
            await _recipeTestService.DeleteRecipeAsync(_sampleData[1].Id);
        }

        [Test]
        public async Task RecipeDbService_UpdateRecipeAsync_Should_Correctly_Update_Recipe()
        {
            var testRecipe = new Recipe
            {
                Id = "7288cb9e-fd10-4843-be36-b2a734216c1b",
                Title = "Test Recipe 1 Updated",
                Description = "Updated description for test recipe 1.",
                RecipeSteps = new List<string> { "Recipe 1 step 1", "Recipe 1 step 2", "Recipe 1 step 3" },
                Notes = "Recipe 1 updated notes.",
                UserId = "ced4bc56-ecd4-4d47-81bb-e74c9406f282"
            };
            await _recipeTestService.AddRecipeAsync(_sampleData[0]);
            await _recipeTestService.UpdateRecipeAsync(testRecipe);
            var result = await _recipeTestService.GetRecipeAsync(testRecipe.Id);
            Assert.That(result.Title, Is.EqualTo(testRecipe.Title));
            Assert.That(result.Description,Is.EqualTo(testRecipe.Description));
            Assert.That(result.RecipeSteps, Is.EqualTo(testRecipe.RecipeSteps));
            Assert.That(result.Notes, Is.EqualTo(testRecipe.Notes));
            Assert.That(result.UserId, Is.EqualTo(testRecipe.UserId));
            await _recipeTestService.DeleteRecipeAsync(testRecipe.Id);
        }

        [Test]
        public async Task
            RecipeDbService_UpdateRecipeAsync_Should_Insert_Recipe_If_Id_Does_Not_Exist_In_Database()
        {
            var testRecipe = new Recipe
            {
                Id = "c4019f0e-f598-465d-b45c-eac2bdddc7f5",
                Title = "Test Recipe",
                Description = "This is a test description for test recipe.",
                RecipeSteps = new List<string> { "Recipe step 1", "Recipe step 2" },
                Notes = "Recipe notes.",
                UserId = "dd943aa0-dcef-4ee5-bf78-d4b55ebeed67"
            };

            await _recipeTestService.UpdateRecipeAsync(testRecipe);
            var result = await _recipeTestService.GetRecipeAsync(testRecipe.Id);
            Assert.That(result.Id, Is.EqualTo(testRecipe.Id));
            await _recipeTestService.DeleteRecipeAsync(testRecipe.Id);
        }
    }
}