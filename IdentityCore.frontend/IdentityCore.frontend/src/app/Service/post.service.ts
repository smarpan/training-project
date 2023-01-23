 import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PostActionModel } from '../Model/PostActionModel';
import { PostModel } from '../Model/PostModel';
import { ResponseApi } from '../Model/ResponseApi';

@Injectable({
  providedIn: 'root'
})
export class PostService {

  constructor(private http: HttpClient) { }
  AddPost(post: PostModel): Observable<ResponseApi> {
    return this.http.post<ResponseApi>("http://localhost:45149/api/Post/addpost", post);
  }
  PostList(): Observable<ResponseApi> {
    return this.http.get<ResponseApi>("http://localhost:45149/api/Post");
  }
  MyPosts(id:string): Observable<ResponseApi> {
    return this.http.get<ResponseApi>("http://localhost:45149/api/Post/profile/" + id);
  }
  DeletePost(model: PostActionModel): Observable<ResponseApi> {
    return this.http.post<ResponseApi>("http://localhost:45149/api/Post/deletemypost", model);
  }
  LikePost(model: PostActionModel): Observable<ResponseApi> {
    return this.http.post<ResponseApi>("http://localhost:45149/api/Post/likepost",model);
  }
}
