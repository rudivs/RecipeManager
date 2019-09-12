import { HttpClient } from "@angular/common/http"
import { Injectable, OnInit } from "@angular/core"
import { map } from "rxjs/operators"
import { Observable } from "rxjs"
import { Recipe, RecipeStep } from "./recipes"

@Injectable()
export class DataService implements OnInit {

    constructor(private http: HttpClient) { }

    public recipes: Recipe[] = [];
    public currentRecipe: Recipe = new Recipe;

    loadRecipes() : Observable<boolean> {
        return this.http.get("/api/recipes")
            .pipe(
            map((data : any[])  =>
        {
            this.recipes = data;
            return true;
        }));
    }

    loadRecipe(recipe: Recipe) {
        this.currentRecipe = recipe;
    }

    ngOnInit(): void {
        if (this.recipes.length > 0) {
            this.currentRecipe = this.recipes[0];
        } 
    }
}