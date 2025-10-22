const LoadingSpinner = () => {
  // Detecta o tema do sistema/localStorage
  const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
  const savedTheme = localStorage.getItem('theme');
  const isDark = savedTheme === 'dark' || (!savedTheme && prefersDark);

  return (
    <div
      style={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        height: '100vh',
        flexDirection: 'column',
        gap: '1.5rem',
        backgroundColor: isDark ? '#242424' : '#f9fafb',
        color: isDark ? '#e5e5e5' : '#1c1c1c',
        transition: 'background-color 0.3s, color 0.3s',
      }}
    >
      <div
        style={{
          width: '50px',
          height: '50px',
          border: '5px solid transparent',
          borderBottom: '5px solid',
          borderBottomColor: isDark ? 'rgba(8, 189, 189, 0.2)' : 'rgba(10, 77, 104, 0.2)',
          borderTop: '5px solid',
          borderTopColor: isDark ? '#08bdbd' : '#0a4d68',
          borderRadius: '50%',
          animation: 'spin 1s linear infinite',
        }}
      />
      <style>
        {`
          @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
          }
        `}
      </style>
      <div
        style={{
          fontSize: '1.1rem',
          fontWeight: '500',
        }}
      >
        Inicializando autenticação...
      </div>
    </div>
  );
};

export default LoadingSpinner;
