import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Lens, LensService } from '../lens.service';

@Component({
  selector: 'app-lens-detail',
  templateUrl: './lens-detail.component.html',
  styleUrls: ['./lens-detail.component.scss']
})
export class LensDetailComponent {

  @Input() lens?: Lens;
  @Output() close = new EventEmitter<void>();
  addedToFavorites = false;

  constructor(
    private lensService: LensService
  ) {}

  addToFavorites() {
    if (!this.lens) {
      return;
    }
    this.lensService.addToFavorites(this.lens);
    this.addedToFavorites = true;
  }

  closeModal() {
    this.close.emit();
  }

}
