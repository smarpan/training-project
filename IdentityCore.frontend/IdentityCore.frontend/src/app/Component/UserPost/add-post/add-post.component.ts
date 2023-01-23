import { Component, ElementRef, ViewChild } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Subscription } from 'rxjs';
import { PostModel } from '../../../Model/PostModel';
import { UserModel } from '../../../Model/UserModel';
import { AuthService } from '../../../Service/auth.service';
import { NotificationService } from '../../../Service/notification.service';
import { PostService } from '../../../Service/post.service';
import { ImageCropperComponent } from '../../segments/image-cropper/image-cropper.component';

@Component({
  selector: 'app-add-post',
  templateUrl: './add-post.component.html',
  styleUrls: ['./add-post.component.css']
})
export class AddPostComponent {
  post: PostModel = new PostModel();
  imagePreview: string = '';
  rawImage: any;

  @ViewChild("fileInput", { static: false })
  inputVar!: ElementRef;

  user: UserModel = {
    id: '',
    userName: '',
    email: '',
    phoneNumber: '',
    gender: '',
    role: '',
    profileImage: '',
    claim: []
  };

  hasAccessToAddPost: boolean = false;
  isAuthenticated: boolean = false;
  loader: boolean = true;

  private loginsubscription!: Subscription;
  private subscriptionHandler!: Subscription;

  constructor(
    private matDialog: MatDialog,
    private authService: AuthService,
    private postService: PostService,
    private notifyService: NotificationService
  ) { }

  ngOnInit() {
    this.loginsubscription = this.authService.loginData.subscribe({
      next: (user) => {
        this.loader = false;
        this.user = user;
        this.hasAccessToAddPost = this.authService.hasAccessToAddPostGetter;
        this.isAuthenticated = this.authService.isAuthenticatedGetter;
      },
      error: (er) => {
        console.log(er.name);
      }
    });

    this.post.userName = this.user.userName;
    this.post.userId = this.user.id;
  }

  fileRawSave(event: any) {
    //console.log("processor is called");
    this.rawImage = event;
    this.subscriptionHandler = this.matDialog.open(ImageCropperComponent, {
      data: { rawImage: this.rawImage, aspectRatio: 4 / 3 },
      hasBackdrop: false
    }).afterClosed().subscribe({
      next: (result) => {
        this.post.image = result;
        this.inputVar.nativeElement.value = "";
        this.imagePreview = "data:image/png;base64," + result;
      },
      error: (er) => {
        console.log(er);
      }
    });
  }

  onSubmit() {

    this.postService.AddPost(this.post).subscribe({
      next: (result) => {
        this.ngOnInit();
        this.notifyService.Notify(result.message);
      },
      error: (er) => {
        this.notifyService.Notify(er.name);
      }
    });
  }

  ngOnDestroy() {
    this.loginsubscription?.unsubscribe();
    this.subscriptionHandler?.unsubscribe();
  }
}
