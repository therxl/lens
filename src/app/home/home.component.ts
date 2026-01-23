import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Lens, LensService } from '../lens.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  allLenses: Lens[] = [];
  filteredLenses: Lens[] = [];

  selectedType: string | null = null;
  selectedLens: Lens | null = null;

  minFocal?: number;
  maxFocal?: number;
  minPrice?: number;
  maxPrice?: number;
  selectedBrand: string = '';

  brands: string[] = [];

  constructor(
    private router: Router,
    private lensService: LensService
  ) {}

  ngOnInit(): void {
    console.log('Home component init');
    // важное место: подгружаем избранное для текущего режима (guest/user)
    this.lensService.refreshFavoritesForCurrentUser();

    this.lensService.getLenses().subscribe({
      next: lenses => {
        console.log('Lenses loaded:', lenses);
        this.allLenses = lenses;
        this.brands = Array.from(new Set(this.allLenses.map(l => l.brand))).sort();
        this.filteredLenses = this.allLenses.filter(l => l.isPopular);
      },
      error: err => {
        console.error('Error loading lenses:', err);
      }
    });

    // Set initial slider values
    this.minFocal = 10;
    this.maxFocal = 500;
    this.minPrice = 1000;
    this.maxPrice = 200000;
  }

  // остальной код без изменений...
  selectType(typeId: string) {
    if (this.selectedType === typeId) {
      this.selectedType = null;
    } else {
      this.selectedType = typeId;
    }
    this.applyFilters();
  }

  resetType() {
    this.selectedType = null;
    this.applyFilters();
  }

  applyFilters() {
    if (this.minFocal != null && this.maxFocal != null && this.minFocal > this.maxFocal) {
      this.minFocal = this.maxFocal;
    }
    if (this.minPrice != null && this.maxPrice != null && this.minPrice > this.maxPrice) {
      this.minPrice = this.maxPrice;
    }

    let list = [...this.allLenses];

    if (this.selectedType) {
      list = list.filter(l => l.type === this.selectedType);
    }
    if (this.minFocal != null) {
      list = list.filter(l => l.maxFocal >= this.minFocal!);
    }
    if (this.maxFocal != null) {
      list = list.filter(l => l.minFocal <= this.maxFocal!);
    }
    if (this.minPrice != null) {
      list = list.filter(l => l.price >= this.minPrice!);
    }
    if (this.maxPrice != null) {
      list = list.filter(l => l.price <= this.maxPrice!);
    }
    if (this.selectedBrand) {
      list = list.filter(l => l.brand === this.selectedBrand);
    }

    const noFilters =
      !this.selectedType &&
      this.minFocal == null &&
      this.maxFocal == null &&
      this.minPrice == null &&
      this.maxPrice == null &&
      !this.selectedBrand;

    this.filteredLenses = noFilters
      ? this.allLenses.filter(l => l.isPopular)
      : list;
  }

  clearFilters() {
    this.selectedType = null;
    this.minFocal = 10;
    this.maxFocal = 500;
    this.minPrice = 1000;
    this.maxPrice = 200000;
    this.selectedBrand = '';
    this.applyFilters();
  }

  openDetails(lens: Lens) {
    this.selectedLens = lens;
  }

  closeModal() {
    this.selectedLens = null;
  }
}
