export interface Task {
  id: string;
  title: string;
  description: string;
  status: 1 | 2 | 3 | 4 | 5; // InPendingUser, InProcess, OnReview, Success, Cancelled
  isAssigned: boolean;
  createdById: string;
  assignedToId?: string;
}