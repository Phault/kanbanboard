import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { LoginComponent } from './login/login.component';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material';

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    RouterModule.forChild([
      { path: 'login', component: LoginComponent },
      // { path: 'register', component: RegisterComponent }
    ]),
    SharedModule,
    MatButtonModule
  ],
  declarations: [
    LoginComponent
  ]
})
export class AuthModule {}
