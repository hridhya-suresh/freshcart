import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {

  const toastr = inject(ToastrService);
  const router = inject(Router);

  return next(req).pipe(

    catchError((error: HttpErrorResponse) => {

      console.error('API Error:', error);

      switch (error.status) {

        case 400:
          toastr.error(
            error.error?.message || 'Invalid request',
            'Bad Request'
          );
          break;

        // case 401:
        //   toastr.error(
        //     'Please sign in to continue',
        //     'Unauthorized'
        //   );

        //   router.navigate(['/signin']);
        //   break;

        case 403:
          toastr.error(
            'You do not have permission to perform this action',
            'Access Denied'
          );
          break;

        case 404:
          toastr.error(
            'The requested resource was not found',
            'Not Found'
          );
          break;

        case 500:
          toastr.error(
            'Something went wrong on the server',
            'Server Error'
          );
          break;

        default:
          toastr.error(
            'Something went wrong. Please try again.',
            'Error'
          );
      }

      return throwError(() => error);

    })

  );

};