import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
   providedIn: 'root',
})
export class PreventUnsavedChangesGuardGuard
   implements CanDeactivate<MemberEditComponent>
{
   canDeactivate(component: MemberEditComponent): boolean {
      if (component.editForm?.dirty)
         // este confirm va a devolver true or false en sus confirmaciones
         return confirm(
            'Are you sure you want to continue?, any unsaved changes will be lost.'
         );

      return true;
   }
}
