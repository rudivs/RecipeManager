import { HttpClient } from "@angular/common/http"
import { Injectable } from "@angular/core"
import { map } from "rxjs/operators"
import { Observable } from "rxjs"
import { Recipe, RecipeStep } from "./recipes"

@Injectable()
export class DataService {

    constructor(private http: HttpClient) { }

    public recipes: Recipe[] = [];

    loadRecipes() : Observable<boolean> {
        return this.http.get("/api/recipes")
            .pipe(
            map((data : any[])  =>
        {
            this.recipes = data;
            return true;
        }));
    }
}