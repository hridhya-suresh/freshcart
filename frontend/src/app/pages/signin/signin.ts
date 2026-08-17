import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-signin',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './signin.html',
  styleUrl: './signin.scss'
})
export class Signin {

  email = '';
  password = '';

  constructor(
    private authService: AuthService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  login() {

    const loginData = {
      email: this.email,
      password: this.password
    };

    this.authService.login(loginData)
      .subscribe({

        next: (response: any) => {

          this.authService.storeLogin(response);

          this.toastr.success('Login Successful');

          this.router.navigate(['/']);
        },

        error: () => {

          this.toastr.error('Invalid Email or Password');

        }

      });
  }
}