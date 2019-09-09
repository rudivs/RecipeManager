using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using RecipeManager.Models;

namespace RecipeManager.Services
{
    public class RecipeDbService : IRecipeDbService
    {
        private readonly Container _container;
        private readonly string _partitionKeyPath = "/id";

        public RecipeDbService(CosmosClient dbClient, string dbName, string recipeContainer)
        {
            var response = dbClient.CreateDatabaseIfNotExistsAsync(dbName).GetAwaiter().GetResult();
            response.Database.CreateContainerIfNotExistsAsync(recipeContainer, _partitionKeyPath).GetAwaiter().GetResult();
            _container = dbClient.GetContainer(dbName, recipeContainer);
        }

        public async Task<List<Recipe>> GetRecipesAsync()
        {
            var iterator =
                _container.GetItemQueryIterator<Recipe>(
                    new QueryDefinition("SELECT * FROM Recipes WHERE Recipes.documentType = 'Recipe'"));
            var results = new List<Recipe>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<Recipe> GetRecipeAsync(string id)
        {
            try
            {
                ItemResponse<Recipe> response = await _container.ReadItemAsync<Recipe>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task AddRecipeAsync(Recipe recipe)
        {
            if (ValidateRecipe(recipe))
            {
                await _container.CreateItemAsync(recipe, new PartitionKey(recipe.Id));
            }
        }

        public async Task UpdateRecipeAsync(Recipe recipe)
        {
            if (ValidateRecipe(recipe))
            {
                await _container.UpsertItemAsync(recipe, new PartitionKey(recipe.Id));
            }
        }

        public async Task DeleteRecipeAsync(string id)
        {
            await _container.DeleteItemAsync<Recipe>(id, new PartitionKey(id));
        }

        private static bool ValidateRecipe(Recipe recipe)
        {
            if (string.IsNullOrEmpty(recipe.Id))
            {
                throw new ArgumentException("Please ensure that the recipe has a valid id.");
            }
            if (string.IsNullOrEmpty(recipe.Title))
            {
                throw new ArgumentException("Please ensure that the recipe has a title.");
            }
            if (recipe.RecipeSteps == null || !recipe.RecipeSteps.Any())
            {
                throw new ArgumentException("Please ensure that the recipe has at least one step.");
            }
            // TODO: throw ArgumentException if userId not in db
            return true;
        }
    }
}
