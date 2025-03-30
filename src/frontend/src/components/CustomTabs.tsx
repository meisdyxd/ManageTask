import { useState, memo } from 'react';
import { ButtonGroup, Button } from 'react-bootstrap';
import { TaskList } from './TaskList';
import { useAuth } from '../contexts/AuthContext';
import { AddTaskModal } from './AddTaskModal';

const CustomTabs = memo(() => {
  const { user } = useAuth();
  const [activeTab, setActiveTab] = useState('pool');
  const [statusFilter, setStatusFilter] = useState<number | null>(null);
  const [showAddTaskModal, setShowAddTaskModal] = useState(false);
  const [refreshKey, setRefreshKey] = useState(0);

  const isManagerOrAdmin = user?.role === 'Manager' || user?.role === 'Admin';

  const handleStatusFilter = (status: number | null) => {
    setStatusFilter(status);
  };

  const handleTaskCreated = () => {
    setRefreshKey(prev => prev + 1);
  };

  const tabStyle = (tabName: string) => ({
    padding: '10px 20px',
    border: 'none',
    backgroundColor: activeTab === tabName ? '#0d6efd' : 'transparent',
    color: activeTab === tabName ? 'white' : '#0d6efd',
    borderRadius: '4px 4px 0 0',
    cursor: 'pointer',
    marginRight: '5px'
  });

  return (
    <div className="task-container">
      <div className="d-flex justify-content-between mb-3 border-bottom">
        <div className="d-flex">
          <button 
            style={tabStyle('pool')}
            onClick={() => setActiveTab('pool')}
          >
            <i className="bi bi-list-check"></i> Общий пул
          </button>
          
          <button 
            style={tabStyle('assigned')}
            onClick={() => setActiveTab('assigned')}
          >
            <i className="bi bi-check-circle"></i> Мои задачи
          </button>
          
          {isManagerOrAdmin && (
            <>
              <button 
                style={tabStyle('all')}
                onClick={() => setActiveTab('all')}
              >
                <i className="bi bi-people"></i> Все задачи
              </button>
              
              <button 
                style={tabStyle('created')}
                onClick={() => setActiveTab('created')}
              >
                <i className="bi bi-pencil"></i> Созданные мной
              </button>
            </>
          )}
        </div>

        {isManagerOrAdmin && (
          <button 
            style={{
              padding: '10px 20px',
              border: 'none',
              backgroundColor: '#28a745',
              color: 'white',
              borderRadius: '4px',
              cursor: 'pointer'
            }}
            onClick={() => setShowAddTaskModal(true)}
          >
            <i className="bi bi-plus-lg"></i> Добавить задачу
          </button>
        )}
      </div>

      <div className="tab-content">
        {activeTab === 'pool' && (
          <TaskList 
            endpoint="task/pool" 
            showStatusFilter={false} 
            key={`pool-${refreshKey}`}
          />
        )}
        
        {activeTab === 'assigned' && (
          <>
            <div className="mb-3">
              <ButtonGroup>
                <Button 
                  variant={statusFilter === null ? 'primary' : 'outline-primary'}
                  onClick={() => handleStatusFilter(null)}
                >
                  Все
                </Button>
                {[1, 2, 3, 4, 5].map(status => (
                  <Button 
                    key={status}
                    variant={statusFilter === status ? 'primary' : 'outline-primary'}
                    onClick={() => handleStatusFilter(status)}
                  >
                    {status === 1 && <><i className="bi bi-clock"></i> Ожидает</>}
                    {status === 2 && <><i className="bi bi-person-workspace"></i> В процессе</>}
                    {status === 3 && <><i className="bi bi-search"></i> На проверке</>}
                    {status === 4 && <><i className="bi bi-check-lg"></i> Завершена</>}
                    {status === 5 && <><i className="bi bi-x-lg"></i> Отменена</>}
                  </Button>
                ))}
              </ButtonGroup>
            </div>
            <TaskList 
              endpoint="task/current" 
              statusFilter={statusFilter} 
              key={`assigned-${refreshKey}`}
            />
          </>
        )}
        
        {activeTab === 'all' && isManagerOrAdmin && (
          <>
            <div className="mb-3">
              <ButtonGroup>
                <Button 
                  variant={statusFilter === null ? 'primary' : 'outline-primary'}
                  onClick={() => handleStatusFilter(null)}
                >
                  Все
                </Button>
                {[1, 2, 3, 4, 5].map(status => (
                  <Button 
                    key={status}
                    variant={statusFilter === status ? 'primary' : 'outline-primary'}
                    onClick={() => handleStatusFilter(status)}
                  >
                    {status === 1 && <><i className="bi bi-clock"></i> Ожидает</>}
                    {status === 2 && <><i className="bi bi-person-workspace"></i> В процессе</>}
                    {status === 3 && <><i className="bi bi-search"></i> На проверке</>}
                    {status === 4 && <><i className="bi bi-check-lg"></i> Завершена</>}
                    {status === 5 && <><i className="bi bi-x-lg"></i> Отменена</>}
                  </Button>
                ))}
              </ButtonGroup>
            </div>
            <TaskList 
              endpoint="task" 
              statusFilter={statusFilter} 
              key={`all-${refreshKey}`}
            />
          </>
        )}
        
        {activeTab === 'created' && isManagerOrAdmin && (
          <>
            <div className="mb-3">
              <ButtonGroup>
                <Button 
                  variant={statusFilter === null ? 'primary' : 'outline-primary'}
                  onClick={() => handleStatusFilter(null)}
                >
                  Все
                </Button>
                {[1, 2, 3, 4, 5].map(status => (
                  <Button 
                    key={status}
                    variant={statusFilter === status ? 'primary' : 'outline-primary'}
                    onClick={() => handleStatusFilter(status)}
                  >
                    {status === 1 && <><i className="bi bi-clock"></i> Ожидает</>}
                    {status === 2 && <><i className="bi bi-person-workspace"></i> В процессе</>}
                    {status === 3 && <><i className="bi bi-search"></i> На проверке</>}
                    {status === 4 && <><i className="bi bi-check-lg"></i> Завершена</>}
                    {status === 5 && <><i className="bi bi-x-lg"></i> Отменена</>}
                  </Button>
                ))}
              </ButtonGroup>
            </div>
            <TaskList 
              endpoint="task/created" 
              statusFilter={statusFilter} 
              key={`created-${refreshKey}`}
            />
          </>
        )}
      </div>

      {isManagerOrAdmin && (
        <AddTaskModal
          show={showAddTaskModal}
          onHide={() => setShowAddTaskModal(false)}
          onTaskCreated={handleTaskCreated}
        />
      )}
    </div>
  );
});

export default CustomTabs;