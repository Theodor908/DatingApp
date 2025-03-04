import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { User } from '../_models/user';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import {setPaginatedResponse, setPaginationHeaders} from './paginationHelper';
import { PaginatedResult } from '../_models/pagination';
import { Photo } from '../_models/photo';

@Injectable({
  providedIn: 'root'
})
export class AdminService {

  baseUrl = environment.apiUrl;
  photoCache = new Map();
  private accountService = inject(AccountService);
  paginatedResult = signal<PaginatedResult<Photo[]> | null>(null);
  user = this.accountService.currentUser();
  private http = inject(HttpClient);
  userParams = signal<UserParams>(new UserParams(this.user));

  getUserWithRoles()
  {
    return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRoles(username: string, roles: string[])
  {
    return this.http.post<string[]>(this.baseUrl + 'admin/edit-roles/' + username + '?roles=' + roles, {});
  }

  getPhotosForApproval()
  {
    const response = this.photoCache.get(Object.values(this.userParams()).join('-'));
    if(response !== undefined)
    {
      return setPaginatedResponse(response, this.paginatedResult);
    }
    let params = setPaginationHeaders(this.userParams().pageNumber, this.userParams().pageSize);
    return this.http.get<Photo[]>(this.baseUrl + 'admin/photos-to-moderate', {observe: 'response', params}).subscribe({
      next: response => {
        setPaginatedResponse(response, this.paginatedResult);
        this.photoCache.set(Object.values(this.userParams()).join('-'), response);
      }
    });
  }

  approvePhoto(photoId: number)
  {
    return this.http.post(this.baseUrl + 'admin/approve-photo/' + photoId, {});
  }

  rejectPhoto(photoId: number)
  {
    return this.http.post(this.baseUrl + 'admin/reject-photo/' + photoId, {});
  }
}
