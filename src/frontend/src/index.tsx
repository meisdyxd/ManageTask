import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import 'bootstrap/dist/css/bootstrap.min.css';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';


const wrapLocalStorage = () => {
  const originalSetItem = localStorage.setItem.bind(localStorage);
  const originalRemoveItem = localStorage.removeItem.bind(localStorage);

  localStorage.setItem = (key: string, value: string) => {
    console.log(`Запись в localStorage: ${key}=${value}`);
    originalSetItem(key, value);
  };

  localStorage.removeItem = (key: string) => {
    console.log(`Удаление из localStorage: ${key}`);
    originalRemoveItem(key);
  };

  window.addEventListener('storage', (event: StorageEvent) => {
    console.log('Изменение в localStorage:', {
      key: event.key,
      oldValue: event.oldValue,
      newValue: event.newValue
    });
  });
};

wrapLocalStorage();

const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);

root.render(
  //<React.StrictMode>
  <>
    <App />
    <ToastContainer position="bottom-right" autoClose={3000} />
  </>
  //</React.StrictMode>
);