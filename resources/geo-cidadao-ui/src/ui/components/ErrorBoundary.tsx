import { Component } from 'react';
import type { ReactNode, ErrorInfo } from 'react';

interface Props {
  children: ReactNode;
}

interface State {
  hasError: boolean;
  error: Error | null;
}

class ErrorBoundary extends Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = {
      hasError: false,
      error: null,
    };
  }

  static getDerivedStateFromError(error: Error): State {
    return {
      hasError: true,
      error,
    };
  }

  componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    console.error('Erro capturado pelo ErrorBoundary:', error, errorInfo);
  }

  handleReload = () => {
    window.location.reload();
  };

  render() {
    if (this.state.hasError) {
      return (
        <div
          style={{
            display: 'flex',
            flexDirection: 'column',
            justifyContent: 'center',
            alignItems: 'center',
            height: '100vh',
            padding: '2rem',
            textAlign: 'center',
            backgroundColor: '#f8f9fa',
          }}
        >
          <div
            style={{
              maxWidth: '600px',
              padding: '2rem',
              backgroundColor: 'white',
              borderRadius: '8px',
              boxShadow: '0 2px 8px rgba(0,0,0,0.1)',
            }}
          >
            <h1 style={{ color: '#dc3545', marginBottom: '1rem' }}>
              ⚠️ Erro de Autenticação
            </h1>
            
            <p style={{ marginBottom: '1.5rem', color: '#666' }}>
              Ocorreu um erro ao inicializar a autenticação.
            </p>

            {this.state.error && (
              <details
                style={{
                  marginBottom: '1.5rem',
                  textAlign: 'left',
                  padding: '1rem',
                  backgroundColor: '#f8f9fa',
                  borderRadius: '4px',
                }}
              >
                <summary style={{ cursor: 'pointer', fontWeight: 'bold' }}>
                  Detalhes do erro
                </summary>
                <pre
                  style={{
                    marginTop: '1rem',
                    fontSize: '0.875rem',
                    overflow: 'auto',
                  }}
                >
                  {this.state.error.toString()}
                </pre>
              </details>
            )}

            <div style={{ display: 'flex', gap: '1rem', justifyContent: 'center' }}>
              <button
                onClick={this.handleReload}
                style={{
                  padding: '0.75rem 1.5rem',
                  backgroundColor: '#007bff',
                  color: 'white',
                  border: 'none',
                  borderRadius: '4px',
                  cursor: 'pointer',
                  fontSize: '1rem',
                }}
              >
                🔄 Recarregar Página
              </button>
              
              <button
                onClick={() => window.location.href = '/'}
                style={{
                  padding: '0.75rem 1.5rem',
                  backgroundColor: '#6c757d',
                  color: 'white',
                  border: 'none',
                  borderRadius: '4px',
                  cursor: 'pointer',
                  fontSize: '1rem',
                }}
              >
                🏠 Ir para Início
              </button>
            </div>

            <div
              style={{
                marginTop: '2rem',
                padding: '1rem',
                backgroundColor: '#fff3cd',
                borderLeft: '4px solid #ffc107',
                borderRadius: '4px',
                textAlign: 'left',
              }}
            >
              <strong>💡 Dica:</strong>
              <ul style={{ marginTop: '0.5rem', paddingLeft: '1.5rem' }}>
                <li>Verifique se o Keycloak está rodando</li>
                <li>Limpe o cache do navegador</li>
                <li>Verifique as variáveis de ambiente (.env)</li>
                <li>Consulte o TROUBLESHOOTING.md para mais ajuda</li>
              </ul>
            </div>
          </div>
        </div>
      );
    }

    return this.props.children;
  }
}

export default ErrorBoundary;
