import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  notifySuccessSubject = new Subject<string>;

  Notify(message: string) {
    this.notifySuccessSubject.next(message);
  }
  constructor() { }

}
