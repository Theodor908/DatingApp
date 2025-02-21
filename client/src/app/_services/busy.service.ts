import { inject, Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {

  busyRequestCount = 0;
  private spinnerService = inject(NgxSpinnerService);
  busy()
  {
    this.busyRequestCount++;
    this.spinnerService.show(undefined, {
      type: 'line-scale-pulse-out-rapid',
      bdColor: 'rgba(255, 255, 255, 0.1)', // bg display color when spinner is active
      color: '#333333' // spinner color
    })
  }

  idle()
  {
    this.busyRequestCount--;
    if(this.busyRequestCount <= 0)
    {
      this.busyRequestCount = 0;
      this.spinnerService.hide(); // turn off spinner
    }
  }
}
