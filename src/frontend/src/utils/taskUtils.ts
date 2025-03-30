export const statusMap: Record<number, string> = {
  1: 'Ожидает',
  2: 'В процессе',
  3: 'На проверке',
  4: 'Завершена',
  5: 'Отменена',
};

export const CheckStatus = (status: number) => {
  switch(status) {
    case 1: return 'primary';
    case 2: return 'info';
    case 3: return 'warning';
    case 4: return 'success';
    case 5: return 'danger';
    default: return 'light';
  }
};