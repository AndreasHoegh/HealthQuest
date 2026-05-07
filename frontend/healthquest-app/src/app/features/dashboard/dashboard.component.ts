import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { QuestService } from '../../core/services/quest.service';
import { Quest } from '../../core/models/models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  quests = signal<Quest[]>([]);
  loading = signal(true);
  error = signal('');
  completingQuestId = signal<string | null>(null);

  // Mock patient data (in real app, fetch from API)
  patientData = {
    name: 'Test Patient',
    level: 5,
    totalPoints: 2450,
    currentStreak: 7,
    complianceRate: 85
  };

  constructor(
    public authService: AuthService,
    private questService: QuestService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadQuests();
  }

  loadQuests(): void {
    this.loading.set(true);
    this.error.set('');

    this.questService.getDailyQuests().subscribe({
      next: (quests) => {
        this.quests.set(quests);
        this.loading.set(false);

        // If no quests, generate them
        if (quests.length === 0) {
          this.generateQuests();
        }
      },
      error: (err) => {
        console.error('Error loading quests:', err);
        
        // If error is because no quests exist, try to generate
        if (err.status === 404 || err.status === 400) {
          this.generateQuests();
        } else {
          this.error.set('Failed to load quests');
          this.loading.set(false);
        }
      }
    });
  }

  generateQuests(): void {
    this.questService.generateDailyQuests().subscribe({
      next: () => {
        // Reload quests after generation
        this.loadQuests();
      },
      error: (err) => {
        console.error('Error generating quests:', err);
        this.error.set('Failed to generate quests');
        this.loading.set(false);
      }
    });
  }

  completeQuest(quest: Quest): void {
    if (quest.isCompleted) return;

    this.completingQuestId.set(quest.id);

    this.questService.completeQuest(quest.id).subscribe({
      next: (updatedQuest) => {
        // Update quest in list
        const currentQuests = this.quests();
        const index = currentQuests.findIndex(q => q.id === quest.id);
        if (index !== -1) {
          currentQuests[index] = updatedQuest;
          this.quests.set([...currentQuests]);
        }

        // Update patient points (mock - in real app, fetch from API)
        this.patientData.totalPoints += quest.points;

        this.completingQuestId.set(null);
      },
      error: (err) => {
        console.error('Error completing quest:', err);
        alert('Failed to complete quest. Please try again.');
        this.completingQuestId.set(null);
      }
    });
  }

  getCompletedCount(): number {
    return this.quests().filter(q => q.isCompleted).length;
  }

  getTotalPoints(): number {
    return this.quests().reduce((sum, q) => sum + q.points, 0);
  }

  getEarnedPoints(): number {
    return this.quests().filter(q => q.isCompleted).reduce((sum, q) => sum + q.points, 0);
  }

  getQuestIcon(quest: Quest): string {
    const icons: { [key: number]: string } = {
      0: '💊', // Medication
      1: '🏃', // Exercise
      2: '📊', // Health Measurement
      3: '👟', // Daily Steps
      4: '💧', // Water Intake
      5: '😴', // Sleep
      6: '⭐'  // Custom
    };
    return icons[quest.type] || '⭐';
  }

  logout(): void {
    this.authService.logout();
  }
}
