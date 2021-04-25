import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Output() cancelled = new EventEmitter();
  userModel: any = {};
  constructor(private accountService: AccountService) { }

  ngOnInit(): void {
  }

  register() {
    console.log(this.userModel)
    this.accountService.register(this.userModel).subscribe(response => {
      this.cancel();

    }, error => {
      console.log(error);
    });
  }

  cancel() {
    this.cancelled.emit(false);
  }
}
