export interface RecipeStep {
    step: string;
    index: string;
}

export interface Recipe {
    id: string;
    Type: string;
    title: string;
    description: string;
    recipeSteps: RecipeStep[];
    notes: string;
    userId: string;
}
