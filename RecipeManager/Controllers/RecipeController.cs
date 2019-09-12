using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> EditAsync([Bind("Id, Title, Description, RecipeSteps, Notes, UserId")]
            Recipe recipe)
        {
            if (!User.Identity.IsAuthenticated || recipe.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return RedirectToAction("Index");
            }

                if (ModelState.IsValid)
            {
                await _recipeDb.UpdateRecipeAsync(recipe);
                return RedirectToAction("View", new { id = recipe.Id });
            }

            return View(recipe);
        }

        [ActionName("Edit")]
        [Authorize]
        public async Task<IActionResult> EditAsync(string id)
        {
            var recipe = await _recipeDb.GetRecipeAsync(id);

            if (!User.Identity.IsAuthenticated || recipe.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return RedirectToAction("Index");
            }

            return View(recipe);
        }

        [ActionName("View")]
        public async Task<IActionResult> ViewAsync(string id)
        {
            var recipe = await _recipeDb.GetRecipeAsync(id);
            return View(recipe);
        }

        [ActionName("Create")]
        [Authorize]
        public IActionResult Create()
        {
            var recipe = new Recipe();
            recipe.RecipeSteps = new List<RecipeStep>{new RecipeStep()};
            return View(recipe);
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult> CreateAsync([Bind("Id,Title,Description,RecipeSteps,Notes,UserId")]
            Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                recipe.Id = Guid.NewGuid().ToString();
                recipe.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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
        [Authorize]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            // first check if the user owns the recipe before deleting it
            var recipe = await _recipeDb.GetRecipeAsync(id);
            if (!User.Identity.IsAuthenticated || recipe.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return RedirectToAction("Index");
            }
            await _recipeDb.DeleteRecipeAsync(id);
            return RedirectToAction("Index");
        }

        [ActionName("Angular")]
        public IActionResult Angular()
        {
            return View();
        }
    }
}
