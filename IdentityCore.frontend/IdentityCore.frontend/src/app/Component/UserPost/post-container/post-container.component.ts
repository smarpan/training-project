import { Component, EventEmitter, HostListener, Input, Output } from '@angular/core';
import { Subscription } from 'rxjs';
import { PostActionModel } from '../../../Model/PostActionModel';
import { PostModel } from '../../../Model/PostModel';
import { AuthService } from '../../../Service/auth.service';
import { ChatService } from '../../../Service/chat.service';
import { NotificationService } from '../../../Service/notification.service';
import { PostService } from '../../../Service/post.service';

@Component({
  selector: 'app-post-container',
  templateUrl: './post-container.component.html',
  styleUrls: ['./post-container.component.css']
})
export class PostContainerComponent {
  @Input() p!: PostModel;
  @Output() postDeleted = new EventEmitter<PostModel>();

  postAction: PostActionModel = {
    id: '',
    userId: ''
  };

  myPost: boolean = false;
  likeStatus: boolean = false;
  alreadyAddedToMessenger: boolean = false;
  likeCount: number = 0;

  private subscriptionHandler1!: Subscription;
  private subscriptionHandler2!: Subscription;

  constructor(
    private chatService: ChatService,
    private postService: PostService,
    private authService: AuthService,
    private notifyService: NotificationService
  ) { }
  ngOnInit(){
   this.alreadyAddedToMessenger = this.chatService.CheckIfContactExists(this.p.userId);
    //console.log(this.alreadyAddedToMessenger+ this.p.userId);
  }
  ngAfterContentInit() {
    this.likeCount = this.p.likes.length;
    if (this.authService.user.id == this.p.userId) {
      this.myPost = true;
    }
    if (this.authService.isAuthenticatedGetter) {
      var check = this.p.likes.indexOf(this.authService.user.id, 0);
      if (check > -1) {
        this.likeStatus = true;
      }
    }
  }

  likeAction() {
    if (this.authService.isAuthenticatedGetter) {
      this.postAction.id = this.p.id;
      this.postAction.userId = this.authService.user.id;
      this.subscriptionHandler1 = this.postService.LikePost(this.postAction).subscribe({
        next: (result) => {
          if (result.status) {
            //this.notifyService.Notify(result.message);
            this.likeStatus = !this.likeStatus;
            if (this.likeStatus) {
              this.likeCount++;
            } else {
              this.likeCount--;
            }
          }
        },
        error: (er) => {
          console.log(er.name);
        }
      });
    } else {
      this.notifyService.Notify("You are not logged in!");
    }
    
  }

  delete(post: PostModel) {
    console.log("post delete is called:- " + post.id);
    if (this.myPost) {
      this.postAction.id = post.id;
      this.postAction.userId = this.authService.user.id;
      this.alreadyAddedToMessenger = this.chatService.CheckIfContactExists(this.p.userId);
      this.subscriptionHandler2 = this.postService.DeletePost(this.postAction).subscribe({
        next: (result) => {
          console.log("Result is recieved");
          this.notifyService.Notify(result.message);
          if (result.status) {
            this.postDeleted.emit(post);
          }
        },
        error: (er) => {
          this.notifyService.Notify(er.name);
        }
      });
    }

  }

  addToMessenger() {
    this.chatService.contactListMiddleware(this.p.userId, this.p.userName);
    this.alreadyAddedToMessenger = true;
  }


  ngOnDestroy() {
    this.subscriptionHandler1?.unsubscribe();
    this.subscriptionHandler2?.unsubscribe();
  }

}
