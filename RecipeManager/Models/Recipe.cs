using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace RecipeManager.Models
{
    public class Recipe
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "Type")]
        public string DocumentType => "Recipe";

        [Required]
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        [Required]
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        [Required]
        [JsonProperty(PropertyName = "recipeSteps")]
        public List<RecipeStep> RecipeSteps { get; set; }

        [JsonProperty(PropertyName = "notes")]
        public string Notes { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }
    }

    public class RecipeStep
    {
        [Required]
        [JsonProperty(PropertyName = "step")]
        public string Step { get; set; }
        public string Index { get; set; }
    }
}
