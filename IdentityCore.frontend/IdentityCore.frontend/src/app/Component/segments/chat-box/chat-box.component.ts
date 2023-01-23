import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AuthService } from '../../../Service/auth.service';
import { ChatService } from '../../../Service/chat.service';
import { MessageStoreModel } from '../../../Model/MessageStoreModel';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-chat-box',
  templateUrl: './chat-box.component.html',
  styleUrls: ['./chat-box.component.css']
})
export class ChatBoxComponent {
  initialGreetings: string = '';
  sendMessage: string = '';
  recieverId: string = '';
  messageStore: MessageStoreModel[] = [];

  private subscriptionHandler1!: Subscription;
  private subscriptionHandler2!: Subscription;

  constructor(
    private dialogRef: MatDialogRef<ChatBoxComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { recieverId: string, recieverUsername: string },
    private authService: AuthService,
    private chatService: ChatService) { }

  ngOnInit() {
    if (this.authService.isAuthenticatedGetter) {
      this.chatService.connectionGetter
        .invoke('Greetings')
        .catch((error: any) => {
          console.log(`Server error: ${error}`);
        });

      this.subscriptionHandler1 = this.chatService.initialGreeting.subscribe({
        next: (result) => {
          this.initialGreetings = result;
        }
      });

      this.subscriptionHandler2 = this.chatService.recievedMessage.subscribe({
        next: (result) => {
          this.messageStore = this.chatService.messageStoreGetter;
        }
      });
    }
  }

  sendMessageFunc() {
    this.chatService.sendMessage(this.data.recieverId, this.data.recieverUsername, this.sendMessage);
  }

  ngOnDestroy() {
    this.subscriptionHandler1?.unsubscribe();
    this.subscriptionHandler2?.unsubscribe();
  }
}
