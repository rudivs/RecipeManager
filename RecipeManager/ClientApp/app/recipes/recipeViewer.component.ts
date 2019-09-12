import { Component, OnInit } from "@angular/core";
import { DataService } from "../shared/dataService";

@Component({
    selector: "recipe-viewer",
    templateUrl: "recipeViewer.component.html",
    styleUrls: []
})
export class RecipeViewer implements OnInit {

    constructor(private data: DataService) { }

    ngOnInit(): void {
        this.data.loadRecipes()
            .subscribe(success => {
                if (success) {
                    this.recipes = this.data.recipes;
                    if (this.data.recipes.length > 0) {
                        this.data.loadRecipe(this.data.recipes[0]);
                    }
                }
            });
    }

}