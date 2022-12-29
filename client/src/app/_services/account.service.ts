import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { User } from '../_models/user';
import { environment } from 'src/environments/environment';

import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
   providedIn: 'root',
})
export class AccountService {
   baseUrl = environment.apiUrl;
   private currentUserSource = new BehaviorSubject<User | null>(null);
   currentUser$ = this.currentUserSource.asObservable(); // p' ocupar el currentUserSource desde afuera del AccountService

   constructor(private http: HttpClient) {}

   login(model: any) {
      return this.http.post<User>(this.baseUrl + 'account/login', model).pipe(
         map((res: User) => {
            const user = res;

            if (user) {
               this.setCurrentUser(user);
            }
         })
      );
   }

   register(model: any) {
      return this.http
         .post<User>(this.baseUrl + 'account/register', model)
         .pipe(
            map((user) => {
               if (user) {
                  this.setCurrentUser(user);
               }

               // return user;
            })
         );
   }

   logout() {
      localStorage.removeItem('user');
      this.currentUserSource.next(null);
   }

   setCurrentUser(user: User) {
      user.roles = [];
      const roles = this.getDecodedToken(user.token).role;
      Array.isArray(roles) ? (user.roles = roles) : user.roles.push(roles);

      localStorage.setItem('user', JSON.stringify(user));
      this.currentUserSource.next(user);
   }

   getDecodedToken(token: string) {
      return JSON.parse(atob(token.split('.')[1]));
   }
}
