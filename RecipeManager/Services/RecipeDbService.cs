using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
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

        public async Task<IEnumerable<Recipe>> GetRecipesAsync()
        {
            throw new NotImplementedException();
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
            await _container.CreateItemAsync(recipe,new PartitionKey(recipe.Id));
        }

        public async Task UpdateRecipeAsync(string id, Recipe recipe)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteRecipeAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}
