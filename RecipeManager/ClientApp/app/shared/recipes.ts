export class RecipeStep {
    step: string;
    index: string;
}

export class Recipe {
    id: string;
    Type: string;
    title: string;
    description: string;
    recipeSteps: Array<RecipeStep> = new Array<RecipeStep>();
    notes: string;
    userId: string;
}
