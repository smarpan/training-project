import { Component } from '@angular/core';
import { PostModel } from '../../../Model/PostModel';
import { PostService } from '../../../Service/post.service';
import { AuthService } from '../../../Service/auth.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-user-profile-page',
  templateUrl: './user-profile-page.component.html',
  styleUrls: ['./user-profile-page.component.css']
})
export class UserProfilePageComponent {
  postList: PostModel[] = [];
  loader: boolean = true;
  userProfile: string='';
  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService,
    private postService: PostService
  ) { }

  ngOnInit() {
      this.userProfile = this.route.snapshot.params['userId'];
      //console.log(this.userProfile);
    
    this.postService.MyPosts(this.userProfile).subscribe({
      next: (result) => {
        this.loader = false;
        if (result.status) {
          this.postList = result.data;
          this.postList.forEach((val) => {
            if (val.image) {
              val.image = "data:image/png;base64," + val.image;
            }

          });
        }
      },
      error: (er) => {
        console.log(er.name);
      }
    });
  }
}
