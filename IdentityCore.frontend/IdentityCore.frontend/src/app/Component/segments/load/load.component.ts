import { Component } from '@angular/core';
import { LoaderService } from '../../../Service/loader.service';

@Component({
  selector: 'app-load',
  templateUrl: './load.component.html',
  styleUrls: ['./load.component.css']
})
export class LoadComponent {
  constructor(public loader: LoaderService) { }
}
