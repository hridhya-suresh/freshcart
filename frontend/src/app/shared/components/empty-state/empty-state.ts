import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
@Component({
  selector: 'app-empty-state',
  imports: [RouterLink],
  templateUrl: './empty-state.html',
  styleUrl: './empty-state.scss',
})
export class EmptyState {
  @Input() icon = '🛒';
  @Input() title = 'Nothing here';
  @Input() message = 'There is nothing to display.';
  @Input() buttonText = 'Continue Shopping';
  @Input() buttonLink = '/';
}
