import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';
import {
   AbstractControl,
   FormBuilder,
   FormControl,
   FormGroup,
   ValidatorFn,
   Validators,
} from '@angular/forms';
import { Router } from '@angular/router';

@Component({
   selector: 'app-register',
   templateUrl: './register.component.html',
   styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
   @Output() cancelRegister = new EventEmitter();
   registerForm: FormGroup = new FormGroup({});
   maxDate: Date = new Date(); // inicializa con fecha y hora actual
   validationErrors: string[] | undefined;

   constructor(
      private accountService: AccountService,
      private toastr: ToastrService,
      private fb: FormBuilder,
      private router: Router
   ) {}

   ngOnInit(): void {
      this.initializeForm();

      this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
   }

   initializeForm() {
      this.registerForm = this.fb.group({
         gender: ['male'],
         username: ['', Validators.required],
         knownAs: ['', Validators.required],
         dateOfBirth: ['', Validators.required],
         city: ['', Validators.required],
         country: ['', Validators.required],
         password: [
            '',
            [
               Validators.required,
               Validators.minLength(4),
               Validators.maxLength(8),
            ],
         ],
         confirmPassword: [
            '',
            [Validators.required, this.matchValues('password')],
         ],
      });

      // por si cambia el password despues de poner el confirmPassword y pasar la validacion
      this.registerForm.controls['password'].valueChanges.subscribe(() => {
         this.registerForm.controls['confirmPassword'].updateValueAndValidity();
      });
   }

   matchValues(matchTo: string): ValidatorFn {
      return (control: AbstractControl) => {
         return control.value === control.parent?.get(matchTo)?.value
            ? null
            : { notMatching: true };
      };
   }

   register() {
      // p' dejar solo dia, mes y año en la date
      const dob = this.getDateOnly(
         this.registerForm.controls['dateOfBirth'].value
      );
      const values = { ...this.registerForm.value, dateOfBirth: dob };

      this.accountService.register(values).subscribe({
         next: (res) => {
            // console.log(res); 🌟
            this.router.navigateByUrl('/members');
         },
         error: (err) => {
            this.validationErrors = err;
         },
      });
   }

   cancel() {
      this.cancelRegister.emit(false);
   }

   private getDateOnly(dob: string | undefined) {
      if (!dob) return;

      let theDob = new Date(dob);

      return new Date(
         theDob.setMinutes(theDob.getMinutes() - theDob.getTimezoneOffset())
      )
         .toISOString()
         .slice(0, 10);
   }
}

// para ver el user ( la res ) necesito return en el map del accountService.register
//
// map((user) => {
//    if (user) {
//       localStorage.setItem('user', JSON.stringify(user));
//       this.currentUserSource.next(user);
//    }
//
//    return user;   🌟
// })
