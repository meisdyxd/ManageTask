// AppRoutes.tsx
import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { useAuth } from './contexts/AuthContext';
import { HomePage } from './pages/HomePage';
import { TasksPage } from './pages/TasksPage';
import { HelpPage } from './pages/HelpPage';
import { AboutPage } from './pages/AboutPage';
import ProfilePage from './pages/ProfilePage';
import UsersPage from './pages/UsersPage';
import { Layout } from './components/Layout';
import { Spinner } from 'react-bootstrap';

export const AppRoutes = () => {
  const { user, loading, isReady } = useAuth();

  if (!isReady || loading) {
    return (
      <div className="d-flex justify-content-center mt-5">
        <Spinner animation="border" role="status">
          <span className="visually-hidden">Загрузка...</span>
        </Spinner>
      </div>
    );
  }

  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<HomePage />} />
        <Route 
          path="/tasks" 
          element={user ? <TasksPage /> : <Navigate to="/" replace />} 
        />
        <Route path="/about" element={<AboutPage />} />
        <Route 
          path="/profile" 
          element={user ? <ProfilePage /> : <Navigate to="/" replace />} 
        />
        <Route 
          path="/users" 
          element={user?.role === 'Admin' ? <UsersPage /> : <Navigate to="/" replace />} 
        />
        <Route path="/help" element={<HelpPage />} />
      </Route>
    </Routes>
  );
};