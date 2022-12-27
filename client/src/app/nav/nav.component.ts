import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
   selector: 'app-nav',
   templateUrl: './nav.component.html',
   styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
   model: any = {};
   // currentUser$: Observable<User | null> = of(null); ✌

   constructor(
      public accountService: AccountService,
      private router: Router,
      private toastr: ToastrService
   ) {}

   ngOnInit(): void {
      // yellow         QUITAR
      this.model = { username: 'lisa', password: 'P@ssword1' };

      // this.currentUser$ = this.accountService.currentUser$;  ✌
   }

   login() {
      // console.log(this.model); {username: 'ariel', password: 'godoy'}

      this.accountService.login(this.model).subscribe({
         next: (res) => {
            // console.log(res);  {username: 'jim', token: '...'}
            this.router.navigateByUrl('/members');

            // yellow p'q al logearme con otro usuario se reseteen los filtros y me vuelva a cargar los del otro sexo ( por ejemplo ), en este componente inyecto el memberService y aca hago un reset de los userParams
         },
         error: (err) => {
            console.log(err);
         },
      });
   }

   logout() {
      this.accountService.logout();
      this.router.navigateByUrl('/');
   }
}
