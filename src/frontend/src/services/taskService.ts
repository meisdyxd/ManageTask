import api from './api';
import { Task } from '../models/Task';

interface TaskResponse {
  items: Task[];
  paginationParams: {
    pageNumber: number;
    pageSize: number;
  };
  hasPreviewPage: boolean;
  hasNextPage: boolean;
  totalPages: number;
}

export const getTasks = async (
  endpoint: string,
  page: number,
  pageSize: number,
  status?: number | null
): Promise<TaskResponse> => {
  const url = `/${endpoint.startsWith('/') ? endpoint.slice(1) : endpoint}`;
  const params = {
    page,
    pageSize,
    ...(status && { status })
  };

  const response = await api.get(url, { params });
  return response.data;
};
export const takeTask = async (taskId: string): Promise<void> => {
  await api.post(`/task/take?taskId=${taskId}`);
};

export const submitTask = async (taskId: string): Promise<void> => {
  await api.post(`/task/check-out?taskId=${taskId}`);
};

export const cancelTask = async (taskId: string): Promise<void> => {
  await api.post(`/task/cancel?taskId=${taskId}`);
};

export const confirmTask = async (taskId: string): Promise<void> => {
  await api.post(`/task/complete?taskId=${taskId}`);
};

export const returnTask = async (taskId: string): Promise<void> => {
  await api.post(`/task/return?taskId=${taskId}`);
};

export const createTask = async (taskData: {
  title: string;
  description: string;
  assignToId?: string;
}): Promise<Task> => {
  const endpoint = taskData.assignToId ? '/task/to-user' : '/task/to-pool';
  
  const requestData = taskData.assignToId 
    ? {
        titile: taskData.title,
        description: taskData.description,
        status: 2, // Для назначенных задач статус "В процессе"
        isAssigned: true,
        assignedToId: taskData.assignToId
      }
    : {
        titile: taskData.title,
        description: taskData.description,
        status: 1, // Для задач в пуле статус "Ожидает"
        isAssigned: false,
        assignedToId: null // Явно указываем null для бэкенда
      };

  const response = await api.post(endpoint, requestData);
  return response.data;
};