import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home/home.component';


const routes: Routes = [];

@NgModule({
  imports: [RouterModule.forRoot(
    [{path: '', component: HomeComponent},
    {path: 'home', component: HomeComponent}]
  )],
  exports: [RouterModule]
})
export class AppRoutingModule { }
