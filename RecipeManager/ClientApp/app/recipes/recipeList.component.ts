import { Component, OnInit } from "@angular/core";
import { DataService } from "../shared/dataService"
import { Recipe, RecipeStep } from "../shared/recipes"

@Component({
    selector: "recipe-list",
    templateUrl: "recipeList.component.html",
    styleUrls:[]
})
export class RecipeList implements OnInit {

    constructor(private data: DataService) {
    }

    public recipes : Recipe[] = [];

    ngOnInit(): void {
        this.data.loadRecipes()
            .subscribe(success => {
                if (success) {
                    this.recipes = this.data.recipes;
                }
            });
    }

    viewRecipe(recipe: Recipe) {
        this.data.loadRecipe(recipe);
    }
}