import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Home4 } from './home4';

describe('Home4', () => {
  let component: Home4;
  let fixture: ComponentFixture<Home4>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Home4]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Home4);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
