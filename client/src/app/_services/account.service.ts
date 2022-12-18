import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs';

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
               localStorage.setItem('user', JSON.stringify(user));
               this.currentUserSource.next(user);
            }
         })
      );
   }

   logout() {
      localStorage.removeItem('user');
      this.currentUserSource.next(null);
   }

   setCurrentUser(user: User) {
      this.currentUserSource.next(user);
   }
}
