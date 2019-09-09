using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeManager.Models;

namespace RecipeManager.Services
{
    public interface IRecipeDbService
    {
        /// <summary>
        /// Get a list of all recipes in the database.
        /// </summary>
        /// <returns>A list of recipes</returns>
        Task<List<Recipe>> GetRecipesAsync();

        /// <summary>
        /// Get a recipe by its id.
        /// </summary>
        /// <param name="id">Id of recipe</param>
        /// <returns>A recipe, or null if the recipe can't be found</returns>
        Task<Recipe> GetRecipeAsync(string id);

        /// <summary>
        /// Add a recipe to the database
        /// </summary>
        /// <param name="recipe">The recipe to be added</param>
        /// <exception cref="Microsoft.Azure.Cosmos.CosmosException">Throws with HttpStatusCode.Conflict when recipe with conflicting id is provided</exception>
        /// <exception cref="ArgumentException">Throws with message 'Please ensure that the recipe has a valid id.' if the provided recipe id is null or empty</exception>
        /// <exception cref="ArgumentException">Throws with message 'Please ensure that the recipe has a title.' if the provided recipe title is null or empty</exception>
        /// <exception cref="ArgumentException">Throws with message 'Please ensure that the recipe has at least one step.' if the provided recipe has no steps</exception>
        Task AddRecipeAsync(Recipe recipe);

        /// <summary>
        /// Update a recipe
        /// </summary>
        /// <param name="recipe">The recipe to be updated. If the recipe does not exist it will be added.</param>
        /// <exception cref="ArgumentException">Throws with message 'Please ensure that the recipe has a valid id.' if the provided recipe id is null or empty</exception>
        /// <exception cref="ArgumentException">Throws with message 'Please ensure that the recipe has a title.' if the provided recipe title is null or empty</exception>
        /// <exception cref="ArgumentException">Throws with message 'Please ensure that the recipe has at least one step.' if the provided recipe has no steps</exception>
        Task UpdateRecipeAsync(Recipe recipe);

        /// <summary>
        /// Delete a recipe with the provided id
        /// </summary>
        /// <param name="id">Id of recipe to delete</param>
        /// <exception cref="Microsoft.Azure.Cosmos.CosmosException">Throws with HttpStatusCode.NotFound when non-existent id is provided</exception>
        Task DeleteRecipeAsync(string id);
    }
}
