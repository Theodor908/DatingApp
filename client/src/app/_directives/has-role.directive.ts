import { Directive, inject, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]' // *appHasRole
})
export class HasRoleDirective implements OnInit{

  @Input() appHasRole: string[] = [];
  private accountService = inject(AccountService);
  private viewContainerRef = inject(ViewContainerRef); // on or more views can be attached to a component
  private templeRef = inject(TemplateRef);

  ngOnInit() : void
  {
    if(this.accountService.roles()?.some(r => this.appHasRole.includes(r)))
    {
      this.viewContainerRef.createEmbeddedView(this.templeRef);
    }
    else
    {
      this.viewContainerRef.clear();
    }
  }

}
