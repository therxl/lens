import { Component } from '@angular/core';
import { Router } from '@angular/router';

interface ShootingType {
  id: string;
  title: string;
  description: string;
}

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent {
  shootingTypes: ShootingType[] = [
    { id: 'portrait', title: 'Портретная съёмка', description: 'Мягкое размытие фона, красивые портреты.' },
    { id: 'landscape', title: 'Пейзажная съёмка', description: 'Широкий угол обзора и высокая детализация.' },
    { id: 'macro', title: 'Макро съёмка', description: 'Съёмка мелких объектов с большим увеличением.' },
    { id: 'sport', title: 'Спортивная съёмка', description: 'Быстрый автофокус и длинное фокусное.' }
  ];

  constructor(private router: Router) {}

  selectType(typeId: string) {
    // переход к списку объективов с выбранным типом съёмки
    this.router.navigate(['/lenses'], {
      queryParams: { shootingType: typeId }
    });
  }
}
