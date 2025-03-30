import { Container } from 'react-bootstrap';
import { Outlet } from 'react-router-dom';
import NavbarComponent from './Navbar'; // Измененный импорт

export const Layout = () => {
  return (
    <>
      <NavbarComponent />
      <Container className="mt-4">
        <Outlet />
      </Container>
    </>
  );
};