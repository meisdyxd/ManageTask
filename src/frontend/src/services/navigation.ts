// navigation.ts
import { useNavigate } from 'react-router-dom';

let navigator: any;

export const setNavigator = (n: any) => {
  navigator = n;
};

export const navigate = (path: string) => {
  if (navigator) {
    navigator(path);
  } else {
    window.location.href = path;
  }
};