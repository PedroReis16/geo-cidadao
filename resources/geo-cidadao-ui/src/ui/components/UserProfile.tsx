import { useAuth } from '../../data/hooks/useAuth';

const UserProfile = () => {
  const { isAuthenticated, username, email, name, logout } = useAuth();

  if (!isAuthenticated) {
    return null;
  }

  return (
    <div style={{ 
      display: 'flex', 
      alignItems: 'center', 
      gap: '1rem',
      padding: '0.5rem 1rem',
      border: '1px solid #ccc',
      borderRadius: '8px'
    }}>
      <div>
        <div style={{ fontWeight: 'bold' }}>
          {name || username || 'Usuário'}
        </div>
        {email && (
          <div style={{ fontSize: '0.875rem', color: '#666' }}>
            {email}
          </div>
        )}
      </div>
      <button 
        onClick={logout}
        style={{
          padding: '0.5rem 1rem',
          backgroundColor: '#dc3545',
          color: 'white',
          border: 'none',
          borderRadius: '4px',
          cursor: 'pointer'
        }}
      >
        Sair
      </button>
    </div>
  );
};

export default UserProfile;
