import { Component, inject, OnInit } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { FormsModule } from '@angular/forms';
import { ButtonsModule } from 'ngx-bootstrap/buttons';

@Component({
  selector: 'app-photo-management',
  imports: [PaginationModule, FormsModule],
  templateUrl: './photo-management.component.html',
  styleUrl: './photo-management.component.css'
})
export class PhotoManagementComponent implements OnInit {
  adminService = inject(AdminService);

  ngOnInit(): void {
    if(this.adminService.paginatedResult() == null)
      this.loadPhotosForApproval();
  }
  loadPhotosForApproval()
  {
    this.adminService.getPhotosForApproval();
  }

  approvePhoto(photoId: number)
  {
    this.adminService.approvePhoto(photoId).subscribe({
      next: () =>  {const paginatedResult = this.adminService.paginatedResult();
        if (paginatedResult && paginatedResult.items) {
          const index = paginatedResult.items.findIndex(p => p.id === photoId);
          if (index !== -1) {
            paginatedResult.items.splice(index, 1);
            this.adminService.paginatedResult.set(paginatedResult);
          }
        }
      }
    });
  }

  rejectPhoto(photoId: number)
  {
    this.adminService.rejectPhoto(photoId).subscribe({
      next: () =>  {const paginatedResult = this.adminService.paginatedResult();
        if (paginatedResult && paginatedResult.items) {
          const index = paginatedResult.items.findIndex(p => p.id === photoId);
          if (index !== -1) {
            paginatedResult.items.splice(index, 1);
            this.adminService.paginatedResult.set(paginatedResult);
          }
        }
      }
    });
  }

  pageChanged(event: any)
  {
    if(this.adminService.userParams().pageNumber !== event.page)
      {
        this.adminService.userParams().pageNumber = event.page;
        this.loadPhotosForApproval();
      }
  }
}
