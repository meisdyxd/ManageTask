import { User } from '../models/User';
import { Button } from 'react-bootstrap';

interface UserCardProps {
  user: User;
  onAssign: (userId: string) => void;
}

export const UserCard = ({ user, onAssign }: UserCardProps) => {
  return (
    <div className="card mb-3">
      <div className="card-body">
        <h5 className="card-title">{user.name}</h5>
        <p className="card-text">
          <small className="text-muted">Email: {user.email}</small><br />
          <small className="text-muted">Роль: {user.role}</small>
        </p>
        <Button 
          variant="primary" 
          size="sm"
          onClick={() => onAssign(user.id)}
        >
          Выбрать
        </Button>
      </div>
    </div>
  );
};