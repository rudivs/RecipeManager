import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from "@angular/common/http"

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { RecipeList } from './recipes/recipeList.component'
import { RecipeViewer } from './recipes/recipeViewer.component'
import { DataService } from "./shared/dataService";

@NgModule({
    declarations: [
        AppComponent,
        RecipeList,
        RecipeViewer
    ],
    imports: [
        BrowserModule,
        AppRoutingModule,
        HttpClientModule
    ],
    providers: [
        DataService
    ],
    bootstrap: [AppComponent]
})
export class AppModule { }
