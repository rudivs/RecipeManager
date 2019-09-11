using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RecipeManager.Models;
using RecipeManager.Services;

namespace RecipeManager.Controllers
{
    [Route("api/recipes")]
    [ApiController]
    [Produces("application/json")]
    public class ApiController : ControllerBase
    {
        private readonly IRecipeDbService _repository;
        private readonly ILogger<ApiController> _logger;

        public ApiController(IRecipeDbService repository, ILogger<ApiController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<List<Recipe>>> Get()
        {
            try
            {
                return await _repository.GetRecipesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to get recipes",e);
                return BadRequest("Unable to get recipes");
            }
        }

    }
}
