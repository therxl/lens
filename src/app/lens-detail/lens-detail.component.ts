import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Lens, LensService } from '../lens.service';

@Component({
  selector: 'app-lens-detail',
  templateUrl: './lens-detail.component.html',
  styleUrls: ['./lens-detail.component.scss']
})
export class LensDetailComponent implements OnInit {

  @Input() lens?: Lens;
  @Output() close = new EventEmitter<void>();
  addedToFavorites = false;

  constructor(
    private lensService: LensService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    if (!this.lens) {
      const id = +this.route.snapshot.paramMap.get('id')!;
      if (id) {
        this.lensService.getLensById(id).subscribe(lens => this.lens = lens);
      }
    }
  }

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
