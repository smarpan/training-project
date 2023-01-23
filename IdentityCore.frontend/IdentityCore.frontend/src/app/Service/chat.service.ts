import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Subject } from 'rxjs';
import { ChatContact } from '../Model/ChatContact';
import { MessageStoreModel } from '../Model/MessageStoreModel';
import { UserService } from './user.service';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubUrl: string = 'http://localhost:45149/api/Chat';
  private currentUserId!: string;
  private currentUserName!: string;

  private connection!: any;
  private connectionEstablished: boolean = false;
  private previousMessagesLoaded: boolean = false;

  showBadge: boolean = false;//beta
  unreadMessageCount: number = 0;//beta

  private messageStore: MessageStoreModel[] = [];
  private temp: MessageStoreModel = new MessageStoreModel();
  private contactList: ChatContact[] = [];

  initialGreeting = new BehaviorSubject<string>('');
  contactUpdate = new BehaviorSubject<boolean>(true);
  recievedMessage = new Subject<boolean>;

  constructor(
    private userService: UserService,) { }

  get connectionEstablishedGetter() { return this.connectionEstablished; }
  get connectionGetter() { return this.connection; }
  get messageStoreGetter() { return this.messageStore; }
  get contactListGetter() { return this.contactList; }

  private async connect(token: string): Promise<void> {
    try {
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(this.hubUrl, { accessTokenFactory: () => token })
        .withAutomaticReconnect()
        .build();
      await this.connection.start();
      this.setSignalrClientMethods();
      this.connectionEstablished = true;
      // console.log(`connection created with connectionId: ${this.connection.connectionId}`);

    } catch (e) {
      console.log(`SignalR connection error: ${e}`);
    }
  }
  private getContactsFunc(userId: string) {
    this.userService.GetContacts(userId).subscribe({
      next: (result) => {
        if (result.status) {
          console.log(result.message);
          console.log(result.data);
          this.contactList = result.data;
          this.contactUpdate.next(true);
        } else {
          console.log(result.message);
        }
      },
      error: (er) => {
        console.log(er);
      }
    });
  }
  private previousMessages(userId: string) {
    this.userService.GetMessagesFromMessageStore(userId).subscribe({
      next: (result) => {
        if (result.status) {
          console.log(result.message);
          this.messageStore = result.data;
          this.recievedMessage.next(true);
          this.previousMessagesLoaded = true;
          this.getMessagesFunc(userId);
        } else {
          this.previousMessagesLoaded = false;
          console.log(result.message);
        }
      }
    });
  }
  private getMessagesFunc(userId: string) {
    if (this.previousMessagesLoaded) {
      console.log("loading pending messages");
      this.userService.GetPendingMessages(userId).subscribe({
        next: (result) => {
          console.log("loaded pending messages");
          if (result.status) {
            console.log(result.data);
            result.data.forEach((x: MessageStoreModel) => {
              this.contactListMiddleware(x.senderId, x.senderUsername);
              this.AddMessageToMessageStore(x.senderId, x.senderUsername, this.currentUserId, this.currentUserName, x.message);
            });

          } else {
            console.log(result.message);
          }
        }
      });
    }

  }
  public initiateConnectionProcess(token: string, userId: string, userName: string) {
    this.currentUserId = userId;
    this.currentUserName = userName;
    this.connect(token);
    this.getContactsFunc(userId);
    this.previousMessages(this.currentUserId);
    //this.getMessagesFunc(userId);

  }
  public exitConnection() {
    this.messageStore = [];
    this.contactList = [];
    this.currentUserId = '';
    this.currentUserName = '';
    this.connectionEstablished = false;
    this.connection.stop();
    this.connection = undefined;
    console.log("exiting connection");
    this.recievedMessage.next(true);
  }

  private setSignalrClientMethods(): void {
    this.connection.on('DisplayGreeting', (message: string) => {
      this.initialGreeting.next(message);
    });
    this.connection.on('RecievedMessage', (senderId: string, senderUsername: string, message: string) => {
      this.AddMessageToMessageStore(senderId, senderUsername, this.currentUserId, this.currentUserName, message);
      this.contactListMiddleware(senderId, senderUsername);
      this.unreadMessageCount++;
      this.showBadge = true;
      console.log("Message is recieved from server");
      this.recievedMessage.next(true);

    });
  }

  CheckIfContactExists(userId: string) {
    var temp = this.contactList.find(f => f.contactUserId == userId);
    if (temp == undefined) {
      return false;
    }
    return true;
  }

  contactListMiddleware(contactUserId: string, contactUserName: string) {
    if (!this.CheckIfContactExists(contactUserId)) {

      this.userService.AddContact(this.currentUserId, contactUserId).subscribe({
        next: (result) => {
          if (result.status) {
            console.log(result.message);
            console.log(result.data);
            this.contactList.push(result.data);
            this.contactUpdate.next(true);
          } else {
            console.log(result.message);
          }

        },
        error: (er) => {
          console.log(er);
        }
      });
      //this.contactList.push({
      //  contactUserId: resultToAdd.id, contactUserName: resultToAdd.userName, contactProfileImage: resultToAdd.profileImage, id: '', userId: this.currentUserId
      //});
    }
  }

  deleteContact(userId: string, contactUserId: string): boolean {

    if (this.CheckIfContactExists(contactUserId)) {
      this.userService.DeleteContact(userId, contactUserId).subscribe({
        next: (result) => {
          console.log(result);
          if (result.status) {
            this.contactList = this.contactList.filter(x => x.contactUserId != contactUserId);
            this.messageStore = this.messageStore.filter(x => x.receiverId != contactUserId && x.senderId != contactUserId);
            this.contactUpdate.next(true);
            console.log("delete contact");
            this.recievedMessage.next(true);
            return true;
          } else {
            return false;
          }
        },
        error: (er) => {
          console.log(er);
        }
      });

    }
    return false;
  }

  private AddMessageToMessageStore(senderId: string, senderUsername: string, receiverId: string, receiverUsername: string, message: string) {
    this.temp.message = message;
    this.temp.receiverId = receiverId;
    this.temp.senderId = senderId;
    this.temp.receiverUsername = receiverUsername;
    this.temp.senderUsername = senderUsername;
    this.temp.BelongsTo = this.currentUserId;
    this.userService.AddToMessageStore(this.currentUserId, this.temp).subscribe({
      next: (result) => {
        if (result.status) {
          console.log(result.message);
          this.messageStore.push({ senderId, senderUsername, receiverId, receiverUsername, message, BelongsTo: this.currentUserId });
          this.recievedMessage.next(true);
        } else {
          console.log(result.message);
        }
      }, error: (er) => {
        console.log(er);
      }
    });

    console.log("this is main message store");
    this.recievedMessage.next(true);
  }

  sendMessage(recieverId: string, recieverUsername: string, message: string) {
    this.AddMessageToMessageStore(this.currentUserId, this.currentUserName, recieverId, recieverUsername, message);
    this.connectionGetter
      .invoke('SendMessageAsync', recieverId, this.currentUserId, this.currentUserName, message)
      .catch((error: any) => {
        console.log(`Server error: ${error}`);

      });

  }
}


