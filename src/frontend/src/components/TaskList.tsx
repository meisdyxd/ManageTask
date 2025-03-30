import { ListGroup, Badge, Pagination, Spinner } from 'react-bootstrap';
import { useEffect, useState } from 'react';
import { getTasks } from '../services/taskService';
import { CheckStatus, statusMap } from '../utils/taskUtils';
import { Task } from '../models/Task';
import { TaskActions } from './TaskActions';

interface TaskListProps {
  endpoint: string;
  statusFilter?: number | null;
  showStatusFilter?: boolean;
}

export const TaskList = ({ 
  endpoint, 
  statusFilter = null,
  showStatusFilter = true 
}: TaskListProps) => {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [pagination, setPagination] = useState({
    page: 1,
    pageSize: 10,
    totalPages: 1,
    hasNext: false,
    hasPrev: false
  });

  const loadTasks = async (page: number) => {
    try {
      setLoading(true);
      const response = await getTasks(endpoint, page, pagination.pageSize, statusFilter);
      
      setTasks(response.items);
      setPagination({
        page: response.paginationParams.pageNumber,
        pageSize: response.paginationParams.pageSize,
        totalPages: response.totalPages,
        hasNext: response.hasNextPage,
        hasPrev: response.hasPreviewPage
      });
    } catch (error) {
      console.error('Ошибка загрузки задач:', error);
      setTasks([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadTasks(1);
  }, [endpoint, statusFilter]);

  const handlePageChange = (page: number) => {
    loadTasks(page);
  };

  if (loading) {
    return (
      <div className="text-center my-5">
        <Spinner animation="border" />
      </div>
    );
  }

  return (
    <div>
      <ListGroup>
        {tasks.map((task) => (
          <ListGroup.Item key={task.id} className="mb-3">
            <div className="d-flex justify-content-between">
              <h5>{task.title}</h5>
              <Badge bg={CheckStatus(task.status)}>
                {statusMap[task.status]}
              </Badge>
            </div>
            <p className="mb-1">{task.description}</p>
            <small className="text-muted">
              {task.isAssigned ? `Назначена: ${task.assignedToId}` : 'Не назначена'}
            </small>
            <TaskActions task={task} onActionComplete={() => loadTasks(pagination.page)} />
          </ListGroup.Item>
        ))}
      </ListGroup>

      {tasks.length === 0 && (
        <div className="text-center my-4">
          <p>Задачи не найдены</p>
        </div>
      )}

      {pagination.totalPages > 1 && (
        <div className="d-flex justify-content-center mt-4">
          <Pagination>
            <Pagination.Prev 
              disabled={!pagination.hasPrev} 
              onClick={() => handlePageChange(pagination.page - 1)}
            />
            
            {Array.from({ length: pagination.totalPages }, (_, i) => i + 1).map(page => (
              <Pagination.Item
                key={page}
                active={page === pagination.page}
                onClick={() => handlePageChange(page)}
              >
                {page}
              </Pagination.Item>
            ))}
            
            <Pagination.Next 
              disabled={!pagination.hasNext} 
              onClick={() => handlePageChange(pagination.page + 1)}
            />
          </Pagination>
        </div>
      )}
    </div>
  );
};