import * as tslib_1 from "tslib";
import { Component } from '@angular/core';
let AppComponent = class AppComponent {
    constructor() {
        this.title = 'Recipe Manager';
    }
};
AppComponent = tslib_1.__decorate([
    Component({
        selector: 'app-root',
        template: `
    <div style="text-align:center" class="content">
      <h1>
        This is Angular
      </h1>
     </div>
    <router-outlet></router-outlet>
  `,
        styles: []
    })
], AppComponent);
export { AppComponent };
//# sourceMappingURL=app.component.js.map