using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using RecipeManager.Models;

namespace RecipeManager.Services
{
    public class RecipeDbService : IRecipeDbService
    {
        private readonly string _dbName;
        private readonly string _collectionName;
        private readonly DocumentClient _client;

        private Uri CollectionUri => UriFactory.CreateDocumentCollectionUri(_dbName, _collectionName);

        public RecipeDbService(DocumentClient dbClient, string dbName, string recipeContainer)
        {
            _dbName = dbName;
            _collectionName = recipeContainer;
            _client = dbClient;
        }

        public async Task<List<Recipe>> GetRecipesAsync()
        {
            var results = _client.CreateDocumentQuery<Recipe>(CollectionUri,sqlExpression: "SELECT * FROM Recipes WHERE Recipes.Type = 'Recipe'").ToList();
            return results;
        }

        public async Task<Recipe> GetRecipeAsync(string id)
        {
            var documentUri = UriFactory.CreateDocumentUri(_dbName, _collectionName, id);
            try
            {
                var response =
                    await _client.ReadDocumentAsync(documentUri);
                return (Recipe)(dynamic)response.Resource;
            }
            catch (DocumentClientException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task AddRecipeAsync(Recipe recipe)
        {
            if (ValidateRecipe(recipe))
            {
                await _client.CreateDocumentAsync(CollectionUri, recipe);
            }
        }

        public async Task UpdateRecipeAsync(Recipe recipe)
        {
            if (ValidateRecipe(recipe))
            {
                await _client.UpsertDocumentAsync(CollectionUri, recipe);
            }
        }

        public async Task DeleteRecipeAsync(string id)
        {
            var documentUri = UriFactory.CreateDocumentUri(_dbName, _collectionName, id);
            await _client.DeleteDocumentAsync(documentUri);
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
