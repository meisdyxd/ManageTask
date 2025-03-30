import React, { useState, useEffect } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { Button, Card, Container, Badge, Row, Col, Spinner, Form, Modal } from 'react-bootstrap';
import { useNavigate, Navigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import { getUsers, updateUser, User } from '../services/userService';

const UsersPage: React.FC = () => {
  const { user, loading, isReady } = useAuth();
  const [users, setUsers] = useState<User[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [pagination, setPagination] = useState({
    page: 1,
    pageSize: 10,
    totalPages: 1
  });
  const [loadingUsers, setLoadingUsers] = useState(true);
  const [showEditModal, setShowEditModal] = useState(false);
  const [editingUser, setEditingUser] = useState<User | null>(null);
  const [formData, setFormData] = useState({
    name: '',
    role: 'User' as 'Admin' | 'Manager' | 'User'
  });
  const navigate = useNavigate();

  useEffect(() => {
    if (isReady && !loading && user?.role === 'Admin') {
      loadUsers(1);
    }
  }, [isReady, loading, user, searchTerm]);

  const loadUsers = async (page: number) => {
    try {
      setLoadingUsers(true);
      const response = await getUsers(page, pagination.pageSize, searchTerm);
      setUsers(response.items);
      setPagination({
        ...pagination,
        page,
        totalPages: response.totalPages
      });
    } catch (error) {
      console.error('Ошибка загрузки пользователей:', error);
      toast.error('Не удалось загрузить пользователей');
    } finally {
      setLoadingUsers(false);
    }
  };

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value);
  };

  const handlePageChange = (page: number) => {
    loadUsers(page);
  };

  const handleEditClick = (user: User) => {
    setEditingUser(user);
    setFormData({
      name: user.name,
      role: user.role
    });
    setShowEditModal(true);
  };

type FormElement = HTMLInputElement | HTMLSelectElement | HTMLTextAreaElement;
const handleFormChange = (e: React.ChangeEvent<FormElement>) => {
  const { name, value } = e.target;
  setFormData(prev => ({
    ...prev,
    [name]: value
  }));
};

  const handleSubmit = async () => {
    if (!editingUser) return;

    try {
      await updateUser(editingUser.id, {
        name: formData.name, // Передаем как "name"
        email: editingUser.email, // Берем из исходных данных
        password: "placeholder", // Любая строка, не будет использоваться
        role: formData.role === 'Admin' ? 1 : 
              formData.role === 'Manager' ? 2 : 3 // Конвертируем в число
      });
      toast.success('Данные пользователя обновлены');
      loadUsers(pagination.page);
      setShowEditModal(false);
    } catch (error) {
      console.error('Ошибка обновления пользователя:', error);
      toast.error('Не удалось обновить данные пользователя');
    }
  };

  const getRoleBadge = (role: string) => {
    switch (role) {
      case 'Admin':
        return <Badge bg="danger">Администратор</Badge>;
      case 'Manager':
        return <Badge bg="warning" text="dark">Менеджер</Badge>;
      default:
        return <Badge bg="secondary">Пользователь</Badge>;
    }
  };

  if (!isReady || loading) {
    return (
      <Container className="d-flex justify-content-center mt-5">
        <Spinner animation="border" />
      </Container>
    );
  }

  if (!user || user.role !== 'Admin') {
    return <Navigate to="/" state={{ from: '/users' }} replace />;
  }

  return (
    <Container className="py-5">
      <h2 className="mb-4">Управление пользователями</h2>

      <Form.Group className="mb-4">
        <Form.Control
          type="text"
          placeholder="Поиск по имени"
          value={searchTerm}
          onChange={handleSearchChange}
        />
      </Form.Group>

      {loadingUsers ? (
        <div className="d-flex justify-content-center mt-5">
          <Spinner animation="border" />
        </div>
      ) : (
        <>
          <Row xs={1} md={2} lg={3} className="g-4">
            {users.map((userItem) => (
              <Col key={userItem.id}>
                <Card className="h-100 shadow-sm">
                  <Card.Body className="d-flex flex-column">
                    <div className="d-flex align-items-center mb-3">
                      <div className="profile-avatar-placeholder rounded-circle bg-light d-flex align-items-center justify-content-center me-3" 
                           style={{ width: '50px', height: '50px' }}>
                        <svg width="20" height="20" viewBox="0 0 24 24" fill="#6c757d">
                          <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"/>
                        </svg>
                      </div>
                      <div>
                        <h5 className="mb-0">{userItem.name}</h5>
                        <small className="text-muted">{userItem.email}</small>
                      </div>
                    </div>
                    
                    <div className="mb-2">
                      <span className="me-2">Роль:</span>
                      {getRoleBadge(userItem.role)}
                    </div>
                    
                    <div className="mt-auto">
                      <Button
                        variant="outline-primary"
                        size="sm"
                        onClick={() => handleEditClick(userItem)}
                        disabled={userItem.role === 'Admin' && userItem.id !== user.id}
                      >
                        Изменить
                      </Button>
                    </div>
                  </Card.Body>
                </Card>
              </Col>
            ))}
          </Row>

          {users.length === 0 && (
            <div className="text-center my-5">
              <p>Пользователи не найдены</p>
            </div>
          )}

          {pagination.totalPages > 1 && (
            <div className="d-flex justify-content-center mt-4">
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

      <Modal show={showEditModal} onHide={() => setShowEditModal(false)} centered>
      <Modal.Header closeButton>
        <Modal.Title>Редактирование пользователя</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <Form>
          <Form.Group className="mb-3">
            <Form.Label>ID</Form.Label>
            <Form.Control plaintext readOnly defaultValue={editingUser?.id} />
          </Form.Group>

          <Form.Group className="mb-3">
            <Form.Label>Email</Form.Label>
            <Form.Control plaintext readOnly defaultValue={editingUser?.email} />
          </Form.Group>

          <Form.Group className="mb-3">
            <Form.Label>Имя</Form.Label>
            <Form.Control
              type="text"
              name="name"
              value={formData.name}
              onChange={handleFormChange}
            />
          </Form.Group>

          <Form.Group className="mb-3">
            <Form.Label>Роль</Form.Label>
            <Form.Select
              name="role"
              value={formData.role}
              onChange={handleFormChange}
              disabled={editingUser?.role === 'Admin' && editingUser.id !== user?.id}
            >
              <option value="User">Пользователь (3)</option>
              <option value="Manager">Менеджер (2)</option>
              {editingUser?.role !== 'Admin' && <option value="Admin">Администратор (1)</option>}
            </Form.Select>
          </Form.Group>
        </Form>
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={() => setShowEditModal(false)}>
          Отмена
        </Button>
        <Button variant="primary" onClick={handleSubmit}>
          Сохранить
        </Button>
      </Modal.Footer>
    </Modal>
    </Container>
  );
};

export default UsersPage;