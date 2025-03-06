import { AfterViewChecked, Component, inject, input, OnDestroy, viewChild, ViewChild } from '@angular/core';
import { MessageService } from '../../_services/message.service';
import { TimeagoModule } from 'ngx-timeago';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-messages',
  imports: [TimeagoModule, FormsModule],
  templateUrl: './member-messages.component.html',
  styleUrl: './member-messages.component.css'
})
export class MemberMessagesComponent implements AfterViewChecked{
  @ViewChild('messageForm') messageForm?: NgForm;
  @ViewChild('scrollMe') scrollContainer?: any;
  messageService = inject(MessageService);
  username = input.required<string>();
  messageContent : string = '';
  loading = false;

  sendMessage()
  {
    this.loading = true;
    this.messageService.sendMessage(this.username(), this.messageContent)?.then(() => {
      this.messageForm?.reset();
      this.scrollToBottom();
    }).finally(() => this.loading = false);
  }

  canDisplayTime(date: Date): boolean
  {
    // if the message has been sent more than a hour ago dont display the time
    return (new Date().getTime() - new Date(date).getTime()) < 3600000;
  }

  ngAfterViewChecked(): void {
    this.scrollToBottom();
  }

  private scrollToBottom(): void {
    if(this.scrollContainer)
    {
      this.scrollContainer.nativeElement.scrollTop = this.scrollContainer.nativeElement.scrollHeight;
    }
  }
}
