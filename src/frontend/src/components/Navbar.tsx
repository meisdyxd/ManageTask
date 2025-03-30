import { memo, useEffect, useState } from 'react';
import { Navbar, Container, Nav, Button } from 'react-bootstrap';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

const NavbarComponent = memo(() => {
  const navigate = useNavigate();
  const { user, signOut } = useAuth();
  const [isClient, setIsClient] = useState(false);

  // Решение проблемы hydration mismatch
  useEffect(() => {
    setIsClient(true);
  }, []);

  const handleLogout = async () => {
    await signOut();
    navigate('/login');
  };

  if (!isClient) {
    return null; // Не рендерить на сервере
  }

  return (
    <Navbar bg="dark" variant="dark" expand="lg">
      <Container>
        <Navbar.Brand as={Link} to="/">Task Manager</Navbar.Brand>
        <Navbar.Toggle aria-controls="basic-navbar-nav" />
        <Navbar.Collapse id="basic-navbar-nav">
          <Nav className="me-auto">
            {user && (
              <>
                <Nav.Link as={Link} to="/tasks">Задачи</Nav.Link>
                {user.role === 'Admin' && (
                  <Nav.Link as={Link} to="/users">Пользователи</Nav.Link>
                )}
              </>
            )}
          </Nav>
          <Nav>
            {user ? (
              <>
                <Nav.Link as={Link} to="/profile">Профиль</Nav.Link>
                <Nav.Link as={Link} to="/about">О нас</Nav.Link>
                <Nav.Link as={Link} to="/help">Помощь</Nav.Link>
              </>
            ) : (
              <>
                <Nav.Link as={Link} to="/about">О нас</Nav.Link>
                <Nav.Link as={Link} to="/help">Помощь</Nav.Link>
              </>
            )}
          </Nav>
        </Navbar.Collapse>
      </Container>
    </Navbar>
  );
});

export default NavbarComponent;