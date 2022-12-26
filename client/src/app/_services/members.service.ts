import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

import { Member } from '../_models/member';
import { map, of } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';

@Injectable({
   providedIn: 'root',
})
export class MembersService {
   baseUrl = environment.apiUrl;
   members: Member[] = [];
   paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();

   constructor(private http: HttpClient) {}

   getMembers(page?: number, itemsPerPage?: number) {
      let params = new HttpParams();

      if (page && itemsPerPage) {
         params = params.append('pageNumber', page);
         params = params.append('pageSize', itemsPerPage);
      }

      // if (this.members.length > 0) return of(this.members);

      // p'q acceder a toda la respuesta y NO solo el body
      return this.http
         .get<Member[]>(this.baseUrl + 'users', { observe: 'response', params })
         .pipe(
            // map((members) => {
            //    this.members = members;

            //    return members;
            // })
            map((res) => {
               if (res.body) {
                  this.paginatedResult.result = res.body;
               }

               // pagination: {"currentPage":1,"itemsPerPage":5,"totalItems":13,"totalPages":3}
               const pagination = res.headers.get('Pagination');

               if (pagination) {
                  this.paginatedResult.pagination = JSON.parse(pagination);
               }

               return this.paginatedResult;
            })
         );
      // Request URL: https:.../api/users?pageNumber=1&pageSize=5
   }

   getMember(username: string) {
      const member = this.members.find((mem) => mem.userName === username);

      if (member) return of(member);

      return this.http.get<Member>(this.baseUrl + 'users/' + username);
   }

   updateMember(member: Member) {
      return this.http.put(this.baseUrl + 'users', member).pipe(
         map(() => {
            const index = this.members.indexOf(member);

            this.members[index] = member;
         })
      );
   }

   setMainPhoto(photoId: number) {
      return this.http.put(
         this.baseUrl + 'users/set-main-photo/' + photoId,
         {}
      );
   }

   deletePhoto(photoId: number) {
      return this.http.delete(
         this.baseUrl + 'users/delete-photo/' + photoId,
         {}
      );
   }
}
