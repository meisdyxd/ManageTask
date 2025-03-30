import React, { createContext, useContext, useEffect, useState, useCallback } from 'react';
import { login, logout, getProfile, register } from '../services/authService';
import axios, { AxiosError } from 'axios';

interface User {
  id: string;
  name: string;
  email: string;
  role: string;
}

interface ApiError {
  errorCode: number;
  message: string;
}

interface AuthContextType {
  user: User | null;
  loading: boolean;
  isReady: boolean;
  error: ApiError[] | null;
  signIn: (email: string, password: string) => Promise<void>;
  signOut: () => Promise<void>;
  signUp: (name: string, email: string, password: string) => Promise<any>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<ApiError[] | null>(null);
  const [isReady, setIsReady] = useState(false);

  const loadUser = useCallback(async () => {
    const token = localStorage.getItem('accessToken');
    if (!token) {
      setLoading(false);
      setIsReady(true);
      setUser(null);
      return;
    }

    try {
      setLoading(true);
      const userData = await getProfile();
      setUser(userData.data);
    } catch (error) {
      setUser(null);
    } finally {
      setLoading(false);
      setIsReady(true);
    }
  }, []);

  useEffect(() => {
    const handleStorageChange = () => {
      loadUser();
    };

    window.addEventListener('storage', handleStorageChange);
    loadUser();

    return () => {
      window.removeEventListener('storage', handleStorageChange);
    };
  }, [loadUser]);

  const signIn = useCallback(async (email: string, password: string) => {
    try {
      setLoading(true);
      setError(null);
      await login({ email, password });
      await loadUser();
    } catch (error: unknown) {
      if (axios.isAxiosError(error)) {
        const errors = error.response?.data as ApiError[];
        setError(errors || [{ errorCode: 500, message: 'Ошибка авторизации' }]);
      } else {
        setError([{ errorCode: 500, message: 'Неизвестная ошибка' }]);
      }
      throw error;
    } finally {
      setLoading(false);
    }
  }, [loadUser]);

  const signUp = useCallback(async (name: string, email: string, password: string) => {
    try {
      setLoading(true);
      setError(null);
      const response = await register({ name, email, password });
      return response;
    } catch (error: unknown) {
      if (axios.isAxiosError(error)) {
        const errors = error.response?.data as ApiError[];
        setError(errors || [{ errorCode: 500, message: 'Ошибка регистрации' }]);
      } else {
        setError([{ errorCode: 500, message: 'Неизвестная ошибка' }]);
      }
      throw error;
    } finally {
      setLoading(false);
    }
  }, []);

  const signOut = useCallback(async () => {
    try {
      setLoading(true);
      await logout();
      localStorage.removeItem('accessToken');
      localStorage.removeItem('refreshToken');
      setUser(null);
      window.dispatchEvent(new Event('storage'));
    } finally {
      setLoading(false);
    }
  }, []);

  const value = {
    user,
    loading,
    isReady,
    error,
    signIn,
    signOut,
    signUp
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};