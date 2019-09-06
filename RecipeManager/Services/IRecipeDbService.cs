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
        Task<IEnumerable<Recipe>> GetRecipesAsync();
        Task<Recipe> GetRecipeAsync(string id);
        Task AddRecipeAsync(Recipe recipe);
        Task UpdateRecipeAsync(string id, Recipe recipe);
        Task DeleteRecipeAsync(string id);
    }
}
