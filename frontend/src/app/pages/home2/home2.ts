import { isPlatformBrowser } from '@angular/common';
import {
  AfterViewInit,
  Component,
  ElementRef,
  inject,
  OnDestroy,
  PLATFORM_ID,
} from '@angular/core';
import {
  destroyFreshcartSliders,
  initFreshcartSliders,
} from '../../shared/freshcart-sliders';

@Component({
  selector: 'app-home2',
  standalone: true,
  imports: [],
  templateUrl: './home2.html',
  styleUrl: './home2.scss',
})
export class Home2 implements AfterViewInit, OnDestroy {
  private readonly platformId = inject(PLATFORM_ID);
  private readonly elementRef = inject(ElementRef<HTMLElement>);

  ngAfterViewInit(): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    initFreshcartSliders(this.elementRef.nativeElement);
  }

  ngOnDestroy(): void {
    if (!isPlatformBrowser(this.platformId)) {
      return;
    }

    destroyFreshcartSliders(this.elementRef.nativeElement);
  }
}
