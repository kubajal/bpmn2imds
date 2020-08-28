import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'bpmn2imds-web-ui';

  addr = "enable js to see my mail";

  ngOnInit(): void {
    this.addr = ["kuba", ".", "jalowiec", "@", "protonmail", ".", "com"].join("");
  }
}
