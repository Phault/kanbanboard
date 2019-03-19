import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../shared/shared.module';
import { RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild([
      { path: 'home', component: HomeComponent },
      { path: '', redirectTo: 'home', pathMatch: 'full' }
    ]),
    SharedModule
  ],
  declarations: [
    HomeComponent
  ]
})
export class DashboardModule { }
