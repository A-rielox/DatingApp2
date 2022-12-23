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

@Component({
   selector: 'app-register',
   templateUrl: './register.component.html',
   styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
   @Output() cancelRegister = new EventEmitter();
   model: any = {};

   registerForm: FormGroup = new FormGroup({});
   maxDate: Date = new Date(); // inicializa con fecha y hora actual

   constructor(
      private accountService: AccountService,
      private toastr: ToastrService,
      private fb: FormBuilder
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
      console.log(this.registerForm.value);

      // this.accountService.register(this.model).subscribe({
      //    next: (res) => {
      //       // console.log(res); 🌟
      //       this.cancel(); // cierro el register form
      //    },
      //    error: (err) => {
      //       console.log(err);
      //       this.toastr.error(err.error + '  💩');
      //    },
      // });
   }

   cancel() {
      this.cancelRegister.emit(false);
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
