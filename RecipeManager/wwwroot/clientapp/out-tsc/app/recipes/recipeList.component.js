import * as tslib_1 from "tslib";
import { Component } from "@angular/core";
let RecipeList = class RecipeList {
    constructor(data) {
        this.data = data;
        this.recipes = [];
    }
    ngOnInit() {
        this.data.loadRecipes()
            .subscribe(success => {
            if (success) {
                this.recipes = this.data.recipes;
            }
        });
    }
    viewRecipe(recipe) {
        this.data.loadRecipe(recipe);
    }
};
RecipeList = tslib_1.__decorate([
    Component({
        selector: "recipe-list",
        templateUrl: "recipeList.component.html",
        styleUrls: []
    })
], RecipeList);
export { RecipeList };
//# sourceMappingURL=recipeList.component.js.map