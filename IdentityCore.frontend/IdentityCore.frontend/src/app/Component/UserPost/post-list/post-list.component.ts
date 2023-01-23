import { Component } from '@angular/core';
import { Subscription } from 'rxjs';
import { PostModel } from '../../../Model/PostModel';
import { PostService } from '../../../Service/post.service';
@Component({
  selector: 'app-post-list',
  templateUrl: './post-list.component.html',
  styleUrls: ['./post-list.component.css']
})
export class PostListComponent {
  postList: PostModel[] = [];
  loader: boolean = true;
  subscriptionHandler1!: Subscription;

  constructor(
    private postService: PostService
  ) { }

  ngOnInit() {
    this.subscriptionHandler1 = this.postService.PostList().subscribe({
      next: (result) => {
        this.loader = false;
        if (result.status) {
          this.postList = result.data;
          console.log(this.postList);
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

  postDeleted(post: PostModel) {
    this.postList = this.postList.filter(x => x.id !== post.id);
  }

  ngOnDestroy() {
    this.subscriptionHandler1?.unsubscribe();
  }
}
