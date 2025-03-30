import { useState } from 'react';
import { Modal, Button, Form, Tabs, Tab } from 'react-bootstrap';
import { createTask } from '../services/taskService';
import { UserSelectionModal } from './UserSelectionModal';

interface AddTaskModalProps {
  show: boolean;
  onHide: () => void;
  onTaskCreated: () => void;
}

export const AddTaskModal = ({ show, onHide, onTaskCreated }: AddTaskModalProps) => {
  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [assignType, setAssignType] = useState<'pool' | 'assign'>('pool');
  const [showUserModal, setShowUserModal] = useState(false);
  const [selectedUserId, setSelectedUserId] = useState<string | null>(null);

  const handleSubmit = async () => {
      try {
        await createTask({
          title,
          description,
          ...(assignType === 'assign' && selectedUserId && { 
            assignToId: selectedUserId 
          })
        });
        onTaskCreated();
        resetForm();
        onHide();
      } catch (error) {
        console.error('Ошибка при создании задачи:', error);
      }
    };

  const resetForm = () => {
    setTitle('');
    setDescription('');
    setAssignType('pool');
    setSelectedUserId(null);
  };

  return (
    <>
      <Modal show={show} onHide={onHide} centered>
        <Modal.Header closeButton>
          <Modal.Title>Добавить новую задачу</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group className="mb-3">
              <Form.Label>Название задачи</Form.Label>
              <Form.Control
                type="text"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                required
              />
            </Form.Group>

            <Form.Group className="mb-3">
              <Form.Label>Описание</Form.Label>
              <Form.Control
                as="textarea"
                rows={3}
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                required
              />
            </Form.Group>

            <Tabs
              activeKey={assignType}
              onSelect={(k) => setAssignType(k as 'pool' | 'assign')}
              className="mb-3"
            >
              <Tab eventKey="pool" title="Добавить в пул">
                <p className="mt-3">Задача будет добавлена в общий пул задач</p>
              </Tab>
              <Tab eventKey="assign" title="Назначить пользователю">
                <div className="mt-3">
                  {selectedUserId ? (
                    <p>Пользователь выбран</p>
                  ) : (
                    <Button 
                      variant="outline-primary"
                      onClick={() => setShowUserModal(true)}
                    >
                      Выбрать пользователя
                    </Button>
                  )}
                </div>
              </Tab>
            </Tabs>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={onHide}>
            Отмена
          </Button>
          <Button 
            variant="primary" 
            onClick={handleSubmit}
            disabled={!title || !description || (assignType === 'assign' && !selectedUserId)}
          >
            Создать задачу
          </Button>
        </Modal.Footer>
      </Modal>

      <UserSelectionModal
        show={showUserModal}
        onHide={() => setShowUserModal(false)}
        onSelect={(userId) => setSelectedUserId(userId)}
      />
    </>
  );
};