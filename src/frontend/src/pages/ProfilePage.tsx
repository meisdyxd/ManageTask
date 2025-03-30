import React from 'react';
import { useAuth } from '../contexts/AuthContext';
import { Button, Card, Container, Badge, Row, Col, Spinner } from 'react-bootstrap';
import { useNavigate, Navigate } from 'react-router-dom';
import { toast } from 'react-toastify';

const ProfilePage: React.FC = () => {
  const { user, loading, isReady, signOut } = useAuth();
  const navigate = useNavigate();
  console.log('User data in Profile:', user); // Добавьте эту строку

  if (!isReady || loading) {
    return (
      <Container className="d-flex justify-content-center mt-5">
        <Spinner animation="border" />
      </Container>
    );
  }

  if (!user) {
    return <Navigate to="/" state={{ from: '/profile' }} replace />;
  }

  const handleCopyId = () => {
    navigator.clipboard.writeText(user.id);
    toast.success('ID скопирован в буфер обмена!');
  };

  const handleLogout = async () => {
    await signOut();
    navigate('/');
  };

  const getRoleBadge = () => {
    switch (user.role) {
      case 'Admin':
        return <Badge bg="danger">Администратор</Badge>;
      case 'Manager':
        return <Badge bg="warning" text="dark">Менеджер</Badge>;
      default:
        return <Badge bg="secondary">Пользователь</Badge>;
    }
  };

  return (
    <Container className="py-5">
      <Row className="justify-content-center">
        <Col md={8} lg={6}>
          <Card className="shadow-sm">
            <Card.Body className="p-4">
              <div className="d-flex flex-column align-items-center text-center mb-4">
                <div className="mb-3 position-relative">
                  <div className="profile-avatar-placeholder rounded-circle bg-light d-flex align-items-center justify-content-center" 
                       style={{ width: '150px', height: '150px' }}>
                    <svg width="60" height="60" viewBox="0 0 24 24" fill="#6c757d" className="text-secondary">
                      <path d="M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z"/>
                    </svg>
                  </div>
                </div>
                
                <h2 className="mb-1">{user.name}</h2>
                <div className="mb-3">
                  {getRoleBadge()}
                </div>
                
                <div className="d-flex align-items-center text-muted mb-2">
                  <span className="me-2">Email:</span>
                  <span>{user.email}</span>
                </div>
                  
                
                <div className="d-flex align-items-center">
                  <span className="me-2">ID:</span>
                  <Button 
                    variant="link" 
                    className="p-0 text-decoration-none d-flex align-items-center"
                    onClick={handleCopyId}
                  >
                    <span className="me-1">Скопировать</span>
                    <svg width="14" height="14" viewBox="0 0 24 24" fill="currentColor">
                      <path d="M16 1H4c-1.1 0-2 .9-2 2v14h2V3h12V1zm3 4H8c-1.1 0-2 .9-2 2v14c0 1.1.9 2 2 2h11c1.1 0 2-.9 2-2V7c0-1.1-.9-2-2-2zm0 16H8V7h11v14z"/>
                    </svg>
                  </Button>
                </div>
              </div>
              
              <div className="d-grid">
                <Button 
                  variant="outline-danger" 
                  onClick={handleLogout}
                  className="d-flex align-items-center justify-content-center"
                >
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor" className="me-2">
                    <path d="M10.09 15.59L11.5 17l5-5-5-5-1.41 1.41L12.67 11H3v2h9.67l-2.58 2.59zM19 3H5c-1.11 0-2 .9-2 2v4h2V5h14v14H5v-4H3v4c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2z"/>
                  </svg>
                  Выйти
                </Button>
              </div>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

export default ProfilePage;