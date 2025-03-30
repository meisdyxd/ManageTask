import axios, { AxiosResponse } from 'axios';

const api = axios.create({
  baseURL: process.env.REACT_APP_API_URL,
  withCredentials: true,
});

let isRefreshing = false;
let failedRequestsQueue: Array<{
  resolve: (token: string) => void;
  reject: (error: any) => void;
}> = [];

api.interceptors.response.use(
  (response) => {
    if (response.config.url?.includes('/account/login')) {
      handleLoginResponse(response);
    }
    return response;
  },
  async (error) => {
    const originalRequest = error.config;
    
    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedRequestsQueue.push({ resolve, reject });
        })
          .then((token) => {
            originalRequest.headers['Authorization'] = `Bearer ${token}`;
            console.log("Обновление рефреш токеном успешно");
            return api(originalRequest);
          })
          .catch((err) => {
            return Promise.reject(err);
          });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        const refreshToken = localStorage.getItem('refreshToken');
        const accessToken = localStorage.getItem('accessToken');
        
        if (!refreshToken || !accessToken) {
          throw new Error('No tokens available');
        }

        const response = await axios.get(
          'https://localhost:7029/api/account/refresh',
          {
            headers: {
              'Authorization': `Bearer ${accessToken}`,
              'Refresh-Token': refreshToken,
            },
            withCredentials: true,
          }
        );

        const authHeader = response.headers['authorization'] || response.headers['Authorization'];
        if (!authHeader) {
          throw new Error('No authorization header in refresh response');
        }

        // Извлекаем токен (удаляем 'Bearer ' если есть)
        const newAccessToken = authHeader.replace(/^Bearer\s+/i, '');

        console.log("Refresh successful, new token:", newAccessToken);
        localStorage.setItem('accessToken', newAccessToken);
        api.defaults.headers.common['Authorization'] = `Bearer ${newAccessToken}`;

        failedRequestsQueue.forEach(({ resolve }) => resolve(newAccessToken));
        failedRequestsQueue = [];

        return api(originalRequest);
      } catch (refreshError) {
        // Clear tokens and notify all components
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        delete api.defaults.headers.common['Authorization'];
        window.dispatchEvent(new Event('storage'));
        
        failedRequestsQueue.forEach(({ reject }) => reject(refreshError));
        failedRequestsQueue = [];

        return Promise.reject(refreshError);
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(error);
  }
);

api.interceptors.request.use((config) => {
  const accessToken = localStorage.getItem('accessToken');
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});

function handleLoginResponse(response: AxiosResponse) {
  const getHeader = (names: string[]): string | null => {
    for (const name of names) {
      const value = response.headers[name];
      if (value) return value;
    }
    return null;
  };

  const accessToken = getHeader(['authorization', 'Authorization']);
  const refreshToken = getHeader(['refresh-token', 'Refresh-Token', 'refresh_token']);

  if (accessToken) {
    const cleanToken = accessToken.replace('Bearer ', '');
    localStorage.setItem('accessToken', cleanToken);
    api.defaults.headers.common['Authorization'] = `Bearer ${cleanToken}`;
  }

  if (refreshToken) {
    localStorage.setItem('refreshToken', refreshToken);
  }
}

export default api;