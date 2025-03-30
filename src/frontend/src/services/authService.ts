import axios from 'axios';
import api from './api';

interface ApiError {
  errorCode: number;
  message: string;
}

export const login = async (data: { email: string; password: string }) => {
  try {
    const response = await api.post('/account/login', data);
    window.dispatchEvent(new Event('storage'));
    return response.data;
  } catch (error: unknown) {
    if (axios.isAxiosError(error)) {
      throw error.response?.data as ApiError[];
    }
    throw [{ errorCode: 500, message: 'Неизвестная ошибка' }];
  }
};

export const register = async (data: { 
  name: string; 
  email: string; 
  password: string 
}) => {
  try {
    const response = await api.post('/account/register', data);
    return response.data;
  } catch (error: unknown) {
    if (axios.isAxiosError(error)) {
      throw error.response?.data as ApiError[];
    }
    throw [{ errorCode: 500, message: 'Неизвестная ошибка' }];
  }
};

export const getProfile = async () => {
  const response = await api.get('/account/profile');
  return response;
};

export const logout = async () => {
  const refreshToken = localStorage.getItem('refreshToken');
  if (!refreshToken) {
    throw new Error('No refresh token available');
  }

  await api.get('/account/logout', {
    headers: {
      'Refresh-Token': refreshToken
    }
  });
  
  localStorage.removeItem('accessToken');
  localStorage.removeItem('refreshToken');
  window.dispatchEvent(new Event('storage'));
};