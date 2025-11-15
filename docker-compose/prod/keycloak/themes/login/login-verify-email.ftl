<#import "template.ftl" as layout>
<@layout.registrationLayout displayInfo=true displayMessage=false bodyClass="verify-email-page"; section>
    <#if section = "header">
        <#-- Header removido para evitar duplicação de informações -->
    <#elseif section = "form">
        <div id="kc-form">
            <div id="kc-form-wrapper">
                <div class="email-verification-container">
                    <div class="email-icon-wrapper">
                        <svg class="email-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/>
                        </svg>
                    </div>
                    
                    <div class="verification-message">
                        <p class="instruction-text">
                            Um email com as instruções para verificar seu endereço de email foi enviada para seu endereço
                        </p>
                        <#if user?? && user.email??>
                            <p class="user-email">${user.email}</p>
                        </#if>
                    </div>

                    <div class="info-box">
                        <div class="info-icon">
                            <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                            </svg>
                        </div>
                        <div class="info-content">
                            <p class="info-title">O que fazer agora?</p>
                            <ul class="info-steps">
                                <li>Verifique sua caixa de entrada</li>
                                <li>Procure também na pasta de spam</li>
                                <li>Clique no link de verificação do email</li>
                                <li>Retorne aqui para fazer login</li>
                            </ul>
                        </div>
                    </div>

                    <p class="click-link-text">
                        Não recebeu um código de verificação no seu email?
                    </p>

                    <p class="resend-link-container">
                        <a href="${url.loginAction}" class="resend-link">Clique aqui</a>
                        <span> para reenviar o código de verificação</span>
                    </p>

                    <div id="auto-check-status" class="auto-check-status" style="display: none;">
                        <div class="spinner"></div>
                        <span>Aguardando verificação do email...</span>
                    </div>

                    <div class="action-buttons">
                        <a href="${url.loginRestartFlowUrl}" class="back-button">
                            <svg class="back-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"/>
                            </svg>
                            Voltar ao login
                        </a>
                    </div>
                </div>
            </div>
        </div>

        <script>
            // Polling para verificar se o email foi verificado
            // Quando o usuário clica no link do email, o Keycloak atualiza o status
            // Este script verifica periodicamente e redireciona automaticamente
            (function() {
                var pollInterval = 3000; // Verifica a cada 3 segundos
                var maxAttempts = 600; // Tenta por 30 minutos (600 * 3s = 1800s)
                var attempts = 0;
                var statusElement = document.getElementById('auto-check-status');

                function showStatus() {
                    if (statusElement) {
                        statusElement.style.display = 'flex';
                    }
                }

                function hideStatus() {
                    if (statusElement) {
                        statusElement.style.display = 'none';
                    }
                }

                function checkEmailVerified() {
                    if (attempts >= maxAttempts) {
                        console.log('Polling parado após atingir o limite de tentativas');
                        hideStatus();
                        return;
                    }

                    attempts++;
                    showStatus();

                    // Faz uma requisição para verificar o status
                    fetch(window.location.href, {
                        method: 'GET',
                        credentials: 'same-origin',
                        headers: {
                            'Accept': 'text/html'
                        }
                    })
                    .then(function(response) {
                        return response.text();
                    })
                    .then(function(html) {
                        // Se a página mudou (não é mais a página de verificação), recarrega
                        if (html.indexOf('email-verification-container') === -1) {
                            window.location.reload();
                        } else {
                            // Continua verificando
                            setTimeout(checkEmailVerified, pollInterval);
                        }
                    })
                    .catch(function(error) {
                        console.error('Erro ao verificar status do email:', error);
                        // Continua verificando mesmo com erro
                        setTimeout(checkEmailVerified, pollInterval);
                    });
                }

                // Inicia a verificação após 5 segundos
                setTimeout(function() {
                    checkEmailVerified();
                }, 5000);
            })();
        </script>
    <#elseif section = "info">
        <#-- Info section vazia para evitar duplicação -->
    </#if>
</@layout.registrationLayout>
