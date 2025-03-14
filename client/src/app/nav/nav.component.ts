import { Component, effect, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { inject } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { HasRoleDirective } from '../_directives/has-role.directive';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-nav',
  imports: [FormsModule, BsDropdownModule, RouterLink, RouterLinkActive, HasRoleDirective],
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css'
})
export class NavComponent{
  accountService =  inject(AccountService);
  messageService = inject(MessageService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  container = 'Inbox';
  pageNumber = 1;
  pageSize = 5;
  model: any = {};

  constructor()
  {
    effect(() => {
      if(this.accountService.currentUser() !== null)
        this.messageService.getUnreadMessagesCount();
    });
  }

  login()
  {
    this.accountService.login(this.model).subscribe({
        next: _ => {
            this.router.navigateByUrl('/members');
        },
        error: error => {
            this.toastr.error(error.error);
        },
        complete: () => {
            console.log('Request has completed');
        }
    });
  }

  logout()
  {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

}
