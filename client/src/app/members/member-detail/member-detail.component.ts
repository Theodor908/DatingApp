import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MembersService } from '../../_services/members.service';
import { Member } from '../../_models/member';
import {TabsModule} from 'ngx-bootstrap/tabs';
import {GalleryModule, GalleryItem, ImageItem} from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { DatePipe } from '@angular/common';
@Component({
  selector: 'app-member-detail',
  imports: [TabsModule, GalleryModule, TimeagoModule, DatePipe],
  templateUrl: './member-detail.component.html',
  styleUrl: './member-detail.component.css'
})
export class MemberDetailComponent implements OnInit{
  private memberService = inject(MembersService);
  private route = inject(ActivatedRoute);
  member?: Member;
  images: GalleryItem[] = [];

  ngOnInit() {
    this.loadMember();
  }

  loadMember() {
    const username = this.route.snapshot.paramMap.get('username'); // get the username from what is sent to this route
    if(username != null)
    {
      this.memberService.getMember(username).subscribe({
        next : member => {this.member = member;
          member.photos.forEach(photo => {
            this.images.push(
              new ImageItem({src: photo?.url, thumb: photo?.url}));
            });
          }
      });
    }
  }

}
