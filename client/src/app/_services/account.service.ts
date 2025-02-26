import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { inject, signal } from '@angular/core';
import { User } from '../_models/user';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { LikesService } from './likes.service';


@Injectable({
  providedIn: 'root'
})
export class AccountService {

  private http = inject(HttpClient);
  private likeService = inject(LikesService); // if it is one way it is okay but dont put accountservice in likesservices because of ciruclar dependency
  baseUrl = environment.apiUrl; // <- important to be accurate
  currentUser = signal<User | null>(null);

  login(model: any)
  {
    return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
      map(user => {
          if(user)
          {
            this.setCurrentUser(user);
          }
        }
      )
    );
  }

  logout()
  {
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }

  register(model: any)
  {
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map(user => {
          if(user)
          {
            this.setCurrentUser(user);
          }
          return user;
        }
      )
    );
  }

  setCurrentUser(user: User)
  {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
    this.likeService.getLikeIds();
  }

}
