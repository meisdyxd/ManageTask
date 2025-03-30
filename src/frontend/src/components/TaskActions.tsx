import { useState } from 'react';
import { Button, Modal } from 'react-bootstrap';
import { 
  takeTask, 
  submitTask, 
  cancelTask, 
  confirmTask, 
  returnTask 
} from '../services/taskService';
import { useAuth } from '../contexts/AuthContext';

interface TaskActionsProps {
  task: {
    id: string;
    title: string;
    description: string;
    status: number;
  };
  onActionComplete: () => void;
}

export const TaskActions = ({ task, onActionComplete }: TaskActionsProps) => {
  const [showModal, setShowModal] = useState(false);
  const [modalContent, setModalContent] = useState({
    title: '',
    message: '',
    action: () => {}
  });
  const { user } = useAuth();

  const handleAction = async (action: () => Promise<void>) => {
    try {
      await action();
      onActionComplete();
    } catch (error) {
      console.error('Ошибка при выполнении действия:', error);
    } finally {
      setShowModal(false);
    }
  };

  const showConfirmation = (title: string, message: string, action: () => Promise<void>) => {
    setModalContent({
      title,
      message,
      action: () => handleAction(action)
    });
    setShowModal(true);
  };

  const renderActions = () => {
    if (!user) return null;

    if (user.role === 'Manager' || user.role === 'Admin') {
      if (task.status === 3) { // На проверке
        return (
          <>
            <Button 
              variant="success" 
              size="sm" 
              onClick={() => showConfirmation(
                'Подтверждение', 
                `Подтвердить выполнение задачи "${task.title}"?`, 
                () => confirmTask(task.id))
              }
            >
              <i className="bi bi-check-lg"></i> Подтвердить
            </Button>
            <Button 
              variant="warning" 
              size="sm" 
              className="ms-2"
              onClick={() => showConfirmation(
                'Возврат задачи', 
                `Вернуть задачу "${task.title}" на доработку?`, 
                () => returnTask(task.id))
              }
            >
              <i className="bi bi-arrow-counterclockwise"></i> Вернуть
            </Button>
            <Button 
              variant="danger" 
              size="sm" 
              className="ms-2"
              onClick={() => showConfirmation(
                'Отмена задачи', 
                `Отменить задачу "${task.title}"?`, 
                () => cancelTask(task.id))
              }
            >
              <i className="bi bi-x-lg"></i> Отменить
            </Button>
          </>
        );
      } else if (task.status === 1 || task.status === 2) { // Ожидает или В процессе
        return (
          <Button 
            variant="danger" 
            size="sm" 
            onClick={() => showConfirmation(
              'Отмена задачи', 
              `Отменить задачу "${task.title}"?`, 
              () => cancelTask(task.id))
            }
          >
            <i className="bi bi-x-lg"></i> Отменить
          </Button>
        );
      }
    } else { // Обычный пользователь
      if (task.status === 1) { // Ожидает
        return (
          <Button 
            variant="primary" 
            size="sm" 
            onClick={() => showConfirmation(
              'Взятие задачи', 
              `Вы уверены, что хотите взять задачу "${task.title}"? Описание: ${task.description}`, 
              () => takeTask(task.id))
            }
          >
            <i className="bi bi-hand-index"></i> Взять задачу
          </Button>
        );
      } else if (task.status === 2) { // В процессе
        return (
          <Button 
            variant="primary" 
            size="sm" 
            onClick={() => showConfirmation(
              'Отправка на проверку', 
              `Отправить задачу "${task.title}" на проверку?`, 
              () => submitTask(task.id))
            }
          >
            <i className="bi bi-send"></i> Отправить на проверку
          </Button>
        );
      }
    }
    return null;
  };

  return (
    <>
      <div className="d-flex justify-content-end mt-2">
        {renderActions()}
      </div>

      <Modal show={showModal} onHide={() => setShowModal(false)} centered>
        <Modal.Header closeButton>
          <Modal.Title>{modalContent.title}</Modal.Title>
        </Modal.Header>
        <Modal.Body>{modalContent.message}</Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowModal(false)}>
            Отмена
          </Button>
          <Button variant="primary" onClick={modalContent.action}>
            Подтвердить
          </Button>
        </Modal.Footer>
      </Modal>
    </>
  );
};