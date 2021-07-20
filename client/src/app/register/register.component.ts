import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
// @Input() userFromHomeComponent :any;
@Output() centelregister =new EventEmitter();
model:any = {};
  constructor(private accountService:AccountService) { }

  ngOnInit(): void {
    // console.log( this.userFromHomeComponent)
  }
register(){
this.accountService.register(this.model).subscribe(Response=>{
  console.log(Response)
  this.cancel();
},error=>{console.log(error);
})
}

cancel(){
  this.centelregister.emit(false);
}
}
