export interface User {
  id: string;
  email: string;
  role: string;
}

export interface AuthResponse {
  token: string;
  userId: string;
  email: string;
  role: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  name: string;
  role: number;
  dateOfBirth?: string;
  condition?: string;
}

export interface Quest {
  id: string;
  patientId: string;
  type: QuestType;
  title: string;
  description: string;
  points: number;
  dueDate: string;
  completedAt?: string;
  isCompleted: boolean;
  verificationData?: string;
  createdAt: string;
}

export enum QuestType {
  Medication = 0,
  Exercise = 1,
  HealthMeasurement = 2,
  DailySteps = 3,
  WaterIntake = 4,
  Sleep = 5,
  Custom = 6
}

export interface Patient {
  id: string;
  userId: string;
  name: string;
  dateOfBirth: string;
  condition: string;
  level: number;
  totalPoints: number;
  currentStreak: number;
  lastActivityDate?: string;
  longestStreak: number;
  complianceRate: number;
  createdAt: string;
  updatedAt: string;
}
