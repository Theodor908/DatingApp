import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

export const authGuard: CanActivateFn = (route, state) => {
  const accountSerivce = inject(AccountService);
  const toastr = inject(ToastrService);

  if(accountSerivce.currentUser())
  {
    return true;
  }
  else
  {
    toastr.error('You shall not pass! (You are not logged in)');
    return false;
  }
};
