import { Component, OnInit } from "@angular/core";
import { DataService } from "../shared/dataService"

@Component({
    selector: "recipe-list",
    templateUrl: "recipeList.component.html",
    styleUrls:[]
})
export class RecipeList implements OnInit {

    constructor(private data: DataService) {
    }

    public recipes = [];

    ngOnInit(): void {
        this.data.loadRecipes()
            .subscribe(success => {
                if (success) {
                    this.recipes = this.data.recipes;
                }
            });
    }
}