import { useState } from 'react';
import { Card, Container, Button, Modal, Form, Alert } from 'react-bootstrap';
import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';

export const HomePage = () => {
  const {user, 
    loading, 
    isReady, 
    signOut, 
    signIn, 
    signUp} = useAuth();
  const navigate = useNavigate();
  
  // Состояния для модальных окон
  const [showLogin, setShowLogin] = useState(false);
  const [showRegister, setShowRegister] = useState(false);
  
  // Данные форм
  const [loginData, setLoginData] = useState({
    email: '',
    password: ''
  });
  
  const [registerData, setRegisterData] = useState({
    name: '',
    email: '',
    password: ''
  });
  
  // Ошибки
  const [loginError, setLoginError] = useState<string | null>(null);
  const [registerError, setRegisterError] = useState<string | null>(null);

  const handleLogin = async () => {
    try {
      setLoginError(null);
      await signIn(loginData.email, loginData.password);
      setShowLogin(false);
      navigate('/tasks');
    } catch (error: any) {
      // Обработка ошибки из API
      if (error.response?.data?.[0]?.message) {
        setLoginError(error.response.data[0].message);
      } else {
        setLoginError('Ошибка авторизации');
      }
    }
  };

  const handleRegister = async () => {
    try {
      setRegisterError(null);
      await signUp(
        registerData.name,
        registerData.email,
        registerData.password
      );
      setShowRegister(false);
      setShowLogin(true); // Переключаем на форму входа
    } catch (error: any) {
      if (error.response?.data?.[0]?.message) {
        setRegisterError(error.response.data[0].message);
      } else {
        setRegisterError('Ошибка регистрации');
      }
    }
  };

  return (
    <Container className="mt-5">
      <Card className="text-center">
        <Card.Header>Добро пожаловать в Task Manager</Card.Header>
        <Card.Body>
          <Card.Title>Управляйте задачами эффективно</Card.Title>
          <Card.Text>
            Система позволяет менеджерам создавать задачи и назначать их сотрудникам.
            Сотрудники могут брать задачи из общего пула и отправлять их на проверку.
          </Card.Text>
          
          {!user ? (
            <div className="d-flex justify-content-center gap-3">
              <Button 
                onClick={() => setShowLogin(true)}
                variant="primary"
              >
                Войти
              </Button>
              <Button 
                onClick={() => setShowRegister(true)}
                variant="outline-primary"
              >
                Регистрация
              </Button>
            </div>
          ) : (
            <Button 
              onClick={() => navigate('/tasks')}
              variant="success"
            >
              Перейти к задачам
            </Button>
          )}
        </Card.Body>
        <Card.Footer className="text-muted">
          Простая и удобная система управления задачами
        </Card.Footer>
      </Card>

      {/* Модальное окно входа */}
      <Modal show={showLogin} onHide={() => setShowLogin(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Вход в систему</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {loginError && <Alert variant="danger">{loginError}</Alert>}
          <Form>
            <Form.Group className="mb-3">
              <Form.Label>Email</Form.Label>
              <Form.Control
                type="email"
                value={loginData.email}
                onChange={(e) => setLoginData({...loginData, email: e.target.value})}
              />
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Пароль</Form.Label>
              <Form.Control
                type="password"
                value={loginData.password}
                onChange={(e) => setLoginData({...loginData, password: e.target.value})}
              />
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowLogin(false)}>
            Отмена
          </Button>
          <Button variant="primary" onClick={handleLogin}>
            Войти
          </Button>
        </Modal.Footer>
      </Modal>

      {/* Модальное окно регистрации */}
      <Modal show={showRegister} onHide={() => setShowRegister(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Регистрация</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          {registerError && <Alert variant="danger">{registerError}</Alert>}
          <Form>
            <Form.Group className="mb-3">
              <Form.Label>Имя пользователя</Form.Label>
              <Form.Control
                value={registerData.name}
                onChange={(e) => setRegisterData({...registerData, name: e.target.value})}
              />
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Email</Form.Label>
              <Form.Control
                type="email"
                value={registerData.email}
                onChange={(e) => setRegisterData({...registerData, email: e.target.value})}
              />
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Пароль</Form.Label>
              <Form.Control
                type="password"
                value={registerData.password}
                onChange={(e) => setRegisterData({...registerData, password: e.target.value})}
              />
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowRegister(false)}>
            Отмена
          </Button>
          <Button variant="primary" onClick={handleRegister}>
            Зарегистрироваться
          </Button>
        </Modal.Footer>
      </Modal>
    </Container>
  );
};