import { Component } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { NgForm } from '@angular/forms';
import { AuthService } from '../../shared/auth.service';

interface Credentials {
  username: string;
  password: string;
}

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {

  isRequesting = false;
  credentials: Credentials;

  constructor(private route: ActivatedRoute, private router: Router, private auth: AuthService) {
    this.clear();
  }

  submit({ valid }: NgForm) {
    if (valid) {
      this.isRequesting = true;
      this.auth.login(this.credentials.username, this.credentials.password)
        .subscribe(
          _token => {
            this.isRequesting = false;
            this.clear();
            this.router.navigate([this.route.snapshot.queryParamMap.get('returnTo') || '/']);
          },
          _err => {
            this.isRequesting = false;
            this.credentials.password = '';
          }
        );
    }
  }

  clear() {
    this.credentials = { username: '', password: ''};
  }

}
