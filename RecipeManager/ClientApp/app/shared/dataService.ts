import { HttpClient } from "@angular/common/http"
import { Injectable } from "@angular/core"
import { map } from "rxjs/operators"

@Injectable()
export class DataService {

    constructor(private http: HttpClient) { }

    public recipes = [];

    loadRecipes() {
        return this.http.get("/api/recipes")
            .pipe(
            map((data : any[])  =>
        {
            this.recipes = data;
            return true;
        }));
    }
}