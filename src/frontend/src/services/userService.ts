import api from './api';

export interface User {
  id: string;
  name: string;
  email: string;
  role: 'Admin' | 'Manager' | 'User';
}

interface UserResponse {
  items: User[];
  paginationParams: {
    pageNumber: number;
    pageSize: number;
  };
  totalPages: number;
}

export const getUsers = async (
  page: number,
  pageSize: number,
  name?: string
): Promise<UserResponse> => {
  const params = {
    page,
    pageSize,
    ...(name && { name })
  };

  const response = await api.get('/user', { params });
  return response.data;
};

export const updateUser = async (id: string, userData: {
  name: string;
  email: string;
  password: string;
  role: number;
}) => {
  const response = await api.put(`/user?id=${id}`, userData);
  return response.data;
};