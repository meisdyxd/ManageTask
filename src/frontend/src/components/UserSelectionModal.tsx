import { useState, useEffect } from 'react';
import { Modal, Button, Form, Row, Col, Spinner } from 'react-bootstrap';
import { getUsers } from '../services/userService';
import { UserCard } from './UserCard';
import { User } from '../models/User';

interface UserSelectionModalProps {
  show: boolean;
  onHide: () => void;
  onSelect: (userId: string) => void;
}

export const UserSelectionModal = ({ show, onHide, onSelect }: UserSelectionModalProps) => {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [pagination, setPagination] = useState({
    page: 1,
    pageSize: 10,
    totalPages: 1
  });
  const [selectedUser, setSelectedUser] = useState<string | null>(null);

  const loadUsers = async (page: number, search: string = '') => {
    try {
      setLoading(true);
      const response = await getUsers(page, pagination.pageSize, search);
      setUsers(response.items);
      setPagination(prev => ({
        ...prev,
        page,
        totalPages: response.totalPages
      }));
    } catch (error) {
      console.error('Ошибка загрузки пользователей:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (show) {
      loadUsers(1);
    }
  }, [show]);

  useEffect(() => {
    const timer = setTimeout(() => {
      loadUsers(1, searchTerm);
    }, 500);

    return () => clearTimeout(timer);
  }, [searchTerm]);

  const handlePageChange = (page: number) => {
    loadUsers(page, searchTerm);
  };

  return (
    <Modal show={show} onHide={onHide} size="lg" centered>
      <Modal.Header closeButton>
        <Modal.Title>Выберите пользователя</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Form.Group className="mb-3">
          <Form.Control
            type="text"
            placeholder="Поиск по имени"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
        </Form.Group>

        {loading ? (
          <div className="text-center">
            <Spinner animation="border" />
          </div>
        ) : (
          <>
            <Row>
              {users.map(user => (
                <Col md={6} key={user.id}>
                  <UserCard 
                    user={user} 
                    onAssign={(userId) => setSelectedUser(userId)}
                  />
                </Col>
              ))}
            </Row>

            {pagination.totalPages > 1 && (
              <div className="d-flex justify-content-center mt-3">
                <Button 
                  variant="outline-primary" 
                  disabled={pagination.page === 1}
                  onClick={() => handlePageChange(pagination.page - 1)}
                  className="me-2"
                >
                  Назад
                </Button>
                <span className="mx-2 align-self-center">
                  Страница {pagination.page} из {pagination.totalPages}
                </span>
                <Button 
                  variant="outline-primary" 
                  disabled={pagination.page === pagination.totalPages}
                  onClick={() => handlePageChange(pagination.page + 1)}
                >
                  Вперед
                </Button>
              </div>
            )}
          </>
        )}
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={onHide}>
          Отмена
        </Button>
        <Button 
          variant="primary" 
          disabled={!selectedUser}
          onClick={() => {
            if (selectedUser) {
              onSelect(selectedUser);
              onHide();
            }
          }}
        >
          Назначить
        </Button>
      </Modal.Footer>
    </Modal>
  );
};