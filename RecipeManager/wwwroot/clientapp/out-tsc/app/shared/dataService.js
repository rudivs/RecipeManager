import * as tslib_1 from "tslib";
import { Injectable } from "@angular/core";
import { map } from "rxjs/operators";
import { Recipe } from "./recipes";
let DataService = class DataService {
    constructor(http) {
        this.http = http;
        this.recipes = [];
        this.currentRecipe = new Recipe;
    }
    loadRecipes() {
        return this.http.get("/api/recipes")
            .pipe(map((data) => {
            this.recipes = data;
            return true;
        }));
    }
    loadRecipe(recipe) {
        this.currentRecipe = recipe;
    }
    ngOnInit() {
        if (this.recipes.length > 0) {
            this.currentRecipe = this.recipes[0];
        }
    }
};
DataService = tslib_1.__decorate([
    Injectable()
], DataService);
export { DataService };
//# sourceMappingURL=dataService.js.map