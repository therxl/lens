import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Lens, LensService } from '../lens.service';

@Component({
  selector: 'app-favorites',
  templateUrl: './favorites.component.html',
  styleUrls: ['./favorites.component.scss']
})
export class FavoritesComponent implements OnInit {

  favorites: Lens[] = [];

  constructor(
    private lensService: LensService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadFavorites();
  }

  loadFavorites(): void {
    this.lensService.getFavorites().subscribe(favorites => {
      this.favorites = favorites;
    });
  }

  openDetails(lens: Lens) {
    this.router.navigate(['/lens', lens.id]);
  }

  removeFromFavorites(lens: Lens) {
    this.lensService.removeFromFavorites(lens).subscribe(() => {
      this.loadFavorites();
    });
  }


}
