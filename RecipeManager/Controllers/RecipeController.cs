using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RecipeManager.Models;
using RecipeManager.Services;

namespace RecipeManager.Controllers
{
    public class RecipeController : Controller
    {
        private readonly IRecipeDbService _recipeDb;

        public RecipeController(IRecipeDbService recipeDbService)
        {
            _recipeDb = recipeDbService;
        }

        [ActionName("Index")]
        public async Task<IActionResult> IndexAsync()
        {
            return View(await _recipeDb.GetRecipesAsync());
        }

        public IActionResult Edit(string id)
        {
            throw new NotImplementedException();
        }

        [ActionName("View")]
        public async Task<IActionResult> ViewAsync(string id)
        {
            var recipe = await _recipeDb.GetRecipeAsync(id);
            return View(recipe);
        }

        [ActionName("Create")]
        public IActionResult Create()
        {
            var recipe = new Recipe();
            recipe.RecipeSteps = new List<RecipeStep>{new RecipeStep()};
            return View(recipe);
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync([Bind("Id,Title,Description,RecipeSteps,Notes,UserId")]
            Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                recipe.Id = Guid.NewGuid().ToString();
                recipe.UserId = Guid.NewGuid().ToString();  //TODO: replace with user id
                await _recipeDb.AddRecipeAsync(recipe);
                return RedirectToAction("Index");
            }

            return View(recipe);
        }

        [ActionName("AddStep")]
        [ResponseCache(NoStore = true, Duration = 0, VaryByHeader = "*")]
        public ActionResult AddStep()
        {
            var recipe = new Recipe();
            recipe.RecipeSteps = new List<RecipeStep> { new RecipeStep() };

            return View(recipe);
        }

        [ActionName("Delete")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            await _recipeDb.DeleteRecipeAsync(id);
            return RedirectToAction("Index");
        }
    }
}
