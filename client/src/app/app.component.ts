import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Felipe Rod Test App';
  users: any;

  constructor(private http:HttpClient){}

  ngOnInit() {
    const r = this.http.get('https://localhost:5001/api/users').subscribe(d=>{this.users=d;});
    //this.http.get('https://localhost:5001/api/users').subscribe({complete:})
    //response => { this.users = response}, 
    //error=> {console.log(error)})
  }
}
