import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';

import { Member } from '../_models/member';
import { map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { User } from '../_models/user';

@Injectable({
   providedIn: 'root',
})
export class MembersService {
   baseUrl = environment.apiUrl;
   members: Member[] = [];
   memberCache = new Map(); // 👉 C : caching
   user: User | undefined;
   userParams: UserParams | undefined;

   constructor(
      private http: HttpClient,
      private accountService: AccountService
   ) {
      this.accountService.currentUser$.pipe(take(1)).subscribe({
         next: (user) => {
            if (user) {
               this.userParams = new UserParams(user);
               this.user = user;
            }
         },
      });
   }

   getUserParams() {
      return this.userParams;
   }

   setUserParams(params: UserParams) {
      this.userParams = params;
   }

   resetUserParams() {
      if (this.user) {
         this.userParams = new UserParams(this.user);

         return this.userParams;
      }

      return;
   }

   // getMembers(page?: number, itemsPerPage?: number) { como ya son muchos parametros => mejor creo un object p' pasarlo aca
   getMembers(userParams: UserParams) {
      const response = this.memberCache.get(
         Object.values(userParams).join('-')
      ); // 👉 C
      if (response) return of(response); // 👉 C

      // si NO esta "cacheado" (// 👉 C) pasa a esto, y al final los guarda en el "cache"
      let params = this.getPaginationHeaders(
         userParams.pageNumber,
         userParams.pageSize
      );

      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);

      return this.getPaginatedResult<Member[]>(
         this.baseUrl + 'users',
         params
      ).pipe(
         // 👉 C
         map((res) => {
            this.memberCache.set(Object.values(userParams).join('-'), res);

            return res;
         })
      );
   }

   getMember(username: string) {
      // 👉 C
      const member = [...this.memberCache.values()]
         .reduce((tot, val) => tot.concat(val.result), [])
         .find((member: Member) => member.userName === username);
      if (member) return of(member);

      // si no esta cacheado
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

   //          LIKES
   // para dar like, el username es de a quien se le da el like
   addLike(username: string) {
      return this.http.post(this.baseUrl + 'likes/' + username, {});
   }

   // GET: api/likes?predicate=liked o likedBy
   // p' agarrar los likes de un user
   getLikes(predicate: string, pageNumber: number, pageSize: number) {
      let params = this.getPaginationHeaders(pageNumber, pageSize);

      params = params.append('predicate', predicate);

      return this.getPaginatedResult<Member[]>(this.baseUrl + 'likes', params);
   }

   //
   //          private
   private getPaginationHeaders(pageNumber: number, pageSize: number) {
      let params = new HttpParams();

      params = params.append('pageNumber', pageNumber);
      params = params.append('pageSize', pageSize);

      return params;
   }

   private getPaginatedResult<T>(url: string, params: HttpParams) {
      const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

      // { observe: 'response' } p'q acceder a toda la respuesta y NO solo el body
      return this.http.get<T>(url, { observe: 'response', params }).pipe(
         map((res) => {
            if (res.body) {
               paginatedResult.result = res.body;
            }

            // pagination: {"currentPage":1,"itemsPerPage":5,"totalItems":13,"totalPages":3}
            const pagination = res.headers.get('Pagination');

            if (pagination) {
               paginatedResult.pagination = JSON.parse(pagination);
            }

            return paginatedResult;
         })
      );
      // Request URL: https:.../api/users?pageNumber=1&pageSize=5
   }
}
