import { Component, inject } from '@angular/core';
import { RegisterComponent } from "../register/register.component";
import { MembersService } from '../_services/members.service';
@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RegisterComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent {

registerMode = false;
memberService = inject(MembersService);

  registerToggle()
  {
    this.registerMode = !this.registerMode;
  }

  cancelRegisterMode(event: boolean)
  {
    this.registerMode = event;
  }
}
