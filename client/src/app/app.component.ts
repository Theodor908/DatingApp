import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { inject } from '@angular/core';
import { OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit{
  http = inject(HttpClient);
  title = 'Russia';
  users: any;
  ngOnInit(): void {
    this.http.get('https://localhost:7111/api/users').subscribe({
      next: response => this.users = response,
      error: error => console.error(error),
      complete: () => console.log('Request has completed')
    });
  }
}
