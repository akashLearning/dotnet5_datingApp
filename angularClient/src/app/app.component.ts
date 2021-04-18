import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'The Dating App';
  users: any;
  constructor(private httpClient: HttpClient) {}

  ngOnInit(): void {
    this.getUsers();
  }
  getUsers() {
    this.httpClient.get('https://localhost:5001/api/users').subscribe(result => {
      this.users = result;
    }, err => console.log(err));
  }
}
