import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { Component } from '@angular/core';
import { Subscription } from 'rxjs';
import { ChatContact } from '../../../Model/ChatContact';
import { LastMessage } from '../../../Model/LastMessage';
import { MessageStoreModel } from '../../../Model/MessageStoreModel';
import { AuthService } from '../../../Service/auth.service';
import { ChatService } from '../../../Service/chat.service';
import { LoaderService } from '../../../Service/loader.service';

@Component({
  selector: 'app-messenger',
  templateUrl: './messenger.component.html',
  styleUrls: ['./messenger.component.css']
})
export class MessengerComponent {
  messageStore: MessageStoreModel[] = [];
  contactList: ChatContact[] = [];
  lastMessage: LastMessage[] = [];
  selectedChat: ChatContact = {
    contactUserId: 'default', contactUserName: '', contactProfileImage: '', id: '', userId: ''
  };
  sendMessage: string = '';
  myId!: string;

  smallScreenLayout: boolean = false;
  contactMessageLayout: boolean = false;
  loader: boolean = true;

  private subscriptionHandler1!: Subscription;
  private subscriptionHandler2!: Subscription;
  private subscriptionHandler3!: Subscription;

  constructor(
    public load: LoaderService,
    private responsive: BreakpointObserver,
    private chatService: ChatService,
    private authService: AuthService) { }

  ngOnInit() {
    this.subscriptionHandler1 = this.responsive.observe(Breakpoints.XSmall).subscribe({
      next: (result) => {
        this.smallScreenLayout = false;
        if (result.matches) {
          this.smallScreenLayout = true;
        }
      }
    });

    this.myId = this.authService.user.id;
    this.contactList = this.chatService.contactListGetter;
    this.messageStore = this.chatService.messageStoreGetter;
    this.messageRead();//beta and needs to be redone.

    this.subscriptionHandler2 = this.chatService.contactUpdate.subscribe({
      next: (result) => {
        this.contactList = this.chatService.contactListGetter;
        this.loader = false;
        this.lastMessageFunc();
      }
    });

    this.subscriptionHandler3 = this.chatService.recievedMessage.subscribe({
      next: (result) => {
        this.messageStore = this.chatService.messageStoreGetter;
        this.lastMessageFunc();
        if (!this.smallScreenLayout || this.selectedChat.contactUserId == 'default') {
          this.messageRead();
        }
      }
    });

  }

  messageRead() { //This feature is beta and currently not available.
    this.chatService.unreadMessageCount = 0;
    this.chatService.showBadge = false;
  }

  resetSelectedChat() {
    this.selectedChat = { contactUserId: 'default', contactUserName: '', contactProfileImage: '', id: '', userId: '' };
  }

  lastMessageFunc() {

    this.contactList.forEach(x => {
      var temp = this.messageStore.filter(y => y.receiverId == x.contactUserId || y.senderId == x.contactUserId).slice(-1);
      if (temp.length > 0) {
        if (temp[0].senderId == this.authService.user.id) {
          this.lastMessage = this.lastMessage.filter(f => f.belongsTo != temp[0].receiverId);
          this.lastMessage.push({ userIdOfLastSender: temp[0].senderId, belongsTo: temp[0].receiverId, userNameToPrint: temp[0].senderUsername, message: temp[0].message });
        }
        if (temp[0].receiverId == this.authService.user.id) {
          this.lastMessage = this.lastMessage.filter(f => f.belongsTo != temp[0].senderId);
          this.lastMessage.push({ userIdOfLastSender: temp[0].senderId, belongsTo: temp[0].senderId, userNameToPrint: temp[0].senderUsername, message: temp[0].message });
        }
      }
    });

  }

  selectChat(chat: ChatContact) {
    this.selectedChat = chat;
  }

  sendMessageFunc() {
    this.chatService.sendMessage(this.selectedChat.contactUserId, this.selectedChat.contactUserName, this.sendMessage);

  }

  delete(contactToRemove: ChatContact) {
    if (this.chatService.deleteContact(contactToRemove.userId, contactToRemove.contactUserId)) {
      this.resetSelectedChat();
    }
  }

  ngOnDestroy() {
    this.subscriptionHandler1?.unsubscribe();
    this.subscriptionHandler2?.unsubscribe();
    this.subscriptionHandler3?.unsubscribe();
  }
}
