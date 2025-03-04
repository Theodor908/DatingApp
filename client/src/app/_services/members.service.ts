import { HttpClient } from '@angular/common/http';
import { inject, Injectable, signal, Signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { Member } from '../_models/member';
import { of } from 'rxjs';
import { Photo } from '../_models/photo';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import {setPaginatedResponse, setPaginationHeaders} from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  private http = inject(HttpClient);
  private accountService = inject(AccountService);
  baseUrl = environment.apiUrl;
  paginatedResult = signal<PaginatedResult<Member[]> | null>(null);
  memberCache = new Map();
  user = this.accountService.currentUser();
  userParams = signal<UserParams>(new UserParams(this.user));

  resetUserParams()
  {
    this.userParams.set(new UserParams(this.user));
  }

  getMembers()
  {
    const response = this.memberCache.get(Object.values(this.userParams()).join('-'));

    if(response !== undefined)
    {
      return setPaginatedResponse(response, this.paginatedResult); // return the cached response
    }

    let params = setPaginationHeaders(this.userParams().pageNumber, this.userParams().pageSize);
    params = params.append('minAge', this.userParams().minAge);
    params = params.append('maxAge', this.userParams().maxAge);
    params = params.append('gender', this.userParams().gender);
    params = params.append('orderBy', this.userParams().orderBy);

    return this.http.get<Member[]>(this.baseUrl + 'users', {observe: 'response', params}).subscribe(
    {
      next: response => {
        setPaginatedResponse(response, this.paginatedResult);
        this.memberCache.set(Object.values(this.userParams()).join('-'), response);
      }
    });
  }

  getMember(username: string)
  {
    const member: Member = [...this.memberCache.values()]
    .reduce((arr, elem) => arr.concat(elem.body), [])
    .find((m : Member) => m.username === username);

    if(member) return of(member);
    return this.http.get<Member>(this.baseUrl + 'users/' + username);
  }

  updateMember(member: Member)
  {
    return this.http.put(this.baseUrl + 'users', member).pipe(
      // tap(() => {
      //   this.members.update(members => members.map(x => x.username === member.username ? member : x)); // find the updated member and update it in the signal members
      // })
    );
  }

  setMainPhoto(photo: Photo)
  {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photo.id, {}).pipe
    (
      // tap(() => {
      //   this.members.update(members => members.map(m => {
      //     if(m.photos.includes(photo))
      //     {
      //       m.photoUrl = photo.url;
      //     }
      //     return m;
      //   }));
      // })
    );
  }

  deletePhoto(photo: Photo)
  {
    return this.http.delete(this.baseUrl + 'users/delete-photo/' + photo.id).pipe(
      // tap(() => {
      //   this.members.update(members => members.map(m => {
      //     if(m.photos.includes(photo))
      //       m.photos = m.photos.filter(p => p.id !== photo.id);
      //     return m;
      //   }));
      // })
    );
  }

}
