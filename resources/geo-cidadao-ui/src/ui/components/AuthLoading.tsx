interface AuthLoadingProps {
  message?: string;
}

/**
 * Componente de loading que respeita o tema da aplicação
 * Usado durante processos de autenticação para manter consistência visual
 */
const AuthLoading = ({ message = 'Carregando...' }: AuthLoadingProps) => {
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
          width: '40px',
          height: '40px',
          border: '4px solid transparent',
          borderTop: '4px solid',
          borderTopColor: isDark ? '#08bdbd' : '#0a4d68',
          borderRadius: '50%',
          animation: 'spin 0.8s linear infinite',
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
      {message && (
        <div
          style={{
            fontSize: '0.9rem',
            fontWeight: '400',
            opacity: 0.8,
          }}
        >
          {message}
        </div>
      )}
    </div>
  );
};

export default AuthLoading;
