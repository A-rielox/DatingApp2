import { Injectable } from '@angular/core';
import { CanActivate } from '@angular/router';
import { Observable, map } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';

@Injectable({
   providedIn: 'root',
})
export class AuthGuard implements CanActivate {
   constructor(
      private accountService: AccountService,
      private toastr: ToastrService
   ) {}

   canActivate(): Observable<boolean> {
      // al pasar un observable es mas sencillo xq el ruter hace la suscripcion y se unsubscribe ( NO necesito el take(1) )
      return this.accountService.currentUser$.pipe(
         // take(1),
         map((user) => {
            if (user) return true;
            else {
               this.toastr.error('🧙‍♂️ You shall not pass!!! 💥⚡⚡💥');
               return false;
            }
         })
      );
   }
}
