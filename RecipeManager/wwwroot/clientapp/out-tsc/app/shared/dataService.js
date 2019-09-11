import * as tslib_1 from "tslib";
import { Injectable } from "@angular/core";
import { map } from "rxjs/operators";
let DataService = class DataService {
    constructor(http) {
        this.http = http;
        this.recipes = [];
    }
    loadRecipes() {
        return this.http.get("/api/recipes")
            .pipe(map((data) => {
            this.recipes = data;
            return true;
        }));
    }
};
DataService = tslib_1.__decorate([
    Injectable()
], DataService);
export { DataService };
//# sourceMappingURL=dataService.js.map