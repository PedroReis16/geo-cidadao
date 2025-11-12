<#import "template.ftl" as layout>
<@layout.registrationLayout displayInfo=true displayMessage=false bodyClass="reset-password-sent-page"; section>
    <#if section = "header">
        <#-- Header removido para evitar duplicação de informações -->
    <#elseif section = "form">
        <div id="kc-form">
            <div id="kc-form-wrapper">
                <div class="reset-confirmation-container">
                    <div class="reset-icon-wrapper">
                        <svg class="reset-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/>
                        </svg>
                    </div>
                    
                    <div class="confirmation-message">
                        <h2 class="confirmation-title">Email enviado!</h2>
                        <p class="instruction-text">
                            Enviamos as instruções para redefinir sua senha para:
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
                            <p class="info-title">Próximos passos:</p>
                            <ul class="info-steps">
                                <li>Verifique sua caixa de entrada</li>
                                <li>Procure também na pasta de spam</li>
                                <li>Clique no link para redefinir sua senha</li>
                                <li>Crie uma nova senha segura</li>
                            </ul>
                        </div>
                    </div>

                    <div class="security-notice">
                        <div class="security-icon">
                            <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"/>
                            </svg>
                        </div>
                        <div class="security-content">
                            <p class="security-title">Segurança</p>
                            <p class="security-text">
                                O link de redefinição é válido por 24 horas e só pode ser usado uma vez.
                            </p>
                        </div>
                    </div>

                    <p class="resend-text">
                        Não recebeu o email?
                        <a href="${url.loginAction}" class="resend-link">Enviar novamente</a>
                    </p>

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
    <#elseif section = "info">
        <#-- Info section vazia para evitar duplicação -->
    </#if>
</@layout.registrationLayout>