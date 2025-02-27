import { Component, inject, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MembersService } from '../../_services/members.service';
import { Member } from '../../_models/member';
import {TabDirective, TabsetComponent, TabsModule} from 'ngx-bootstrap/tabs';
import {GalleryModule, GalleryItem, ImageItem} from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
import { MemberMessagesComponent } from "../member-messages/member-messages.component";
import { Message } from '../../_models/message';
import { MessageService } from '../../_services/message.service';
@Component({
  selector: 'app-member-detail',
  imports: [TabsModule, GalleryModule, TimeagoModule, DatePipe, MemberMessagesComponent],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css'
})
export class MemberDetailComponent implements OnInit{
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent; // member tabs available while view is created
  private messageService = inject(MessageService);
  private memberService = inject(MembersService);
  private route = inject(ActivatedRoute);
  member: Member = {} as Member;
  images: GalleryItem[] = [];
  activeTab?: TabDirective;
  messages: Message[] = [];

  ngOnInit() {
    this.route.data.subscribe(
      {
        next: data => {this.member = data['member'];
          this.member && this.member.photos.forEach(photo => {
            this.images.push(
              new ImageItem({src: photo?.url, thumb: photo?.url}));
            });
          }
      }
    );
    this.route.queryParams.subscribe({
      next: params => {
        params['tab'] && this.selectTab(params['tab']);
      }
    });
  }

  selectTab(heading: string)
  {
    if(this.memberTabs)
    {
      const messageTab = this.memberTabs.tabs.find(x => x.heading === heading);
      if(messageTab)
        messageTab.active = true;
    }
  }

  onUpdateMessages(event : Message)
  {
    this.messages.push(event);
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if(this.activeTab.heading === 'Messages' && this.messages.length === 0 && this.member)
    {
      this.messageService.getMessageThread(this.member.username)
      .subscribe({
        next: messages => this.messages = messages
      });
    }
  }

  loadMember() {
    // const username = this.route.snapshot.paramMap.get('username'); // get the username from what is sent to this route
    // if(username != null)
    // {
    //   this.memberService.getMember(username).subscribe({
    //     next : member => {this.member = member;

    //   });
    // }
  }
}
