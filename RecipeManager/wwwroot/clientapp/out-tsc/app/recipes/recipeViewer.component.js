import * as tslib_1 from "tslib";
import { Component } from "@angular/core";
let RecipeViewer = class RecipeViewer {
    constructor(data) {
        this.data = data;
    }
    ngOnInit() {
        this.data.loadRecipes()
            .subscribe(success => {
            if (success) {
                if (this.data.recipes.length > 0) {
                    this.data.loadRecipe(this.data.recipes[0]);
                }
            }
        });
    }
};
RecipeViewer = tslib_1.__decorate([
    Component({
        selector: "recipe-viewer",
        templateUrl: "recipeViewer.component.html",
        styleUrls: []
    })
], RecipeViewer);
export { RecipeViewer };
//# sourceMappingURL=recipeViewer.component.js.map