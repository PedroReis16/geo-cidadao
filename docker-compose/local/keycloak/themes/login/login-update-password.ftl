<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=false bodyClass="update-password-page"; section>
    <#if section = "header">
        <div class="logo-container">
            <img id="kc-logo-img" src="${url.resourcesPath}/img/logo-light.png" alt="GeoCidadão">
            <h1 class="kc-logo-text">Nova senha</h1>
            <p>Crie uma nova senha segura para sua conta</p>
        </div>
    <#elseif section = "form">
        <div id="kc-form">
            <div id="kc-form-wrapper">
                <form id="kc-passwd-update-form" class="${properties.kcFormClass!}" action="${url.loginAction}" method="post">
                    
                    <div class="password-info-box">
                        <div class="password-icon">
                            <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"/>
                            </svg>
                        </div>
                        <div class="password-info-content">
                            <p class="password-info-title">Dicas para uma senha segura:</p>
                            <ul class="password-tips">
                                <li>Use pelo menos 8 caracteres</li>
                                <li>Combine letras, números e símbolos</li>
                                <li>Evite informações pessoais</li>
                                <li>Use uma senha única</li>
                            </ul>
                        </div>
                    </div>

                    <div class="password-fields">
                        <!-- Nova senha -->
                        <div class="form-group ${messagesPerField.printIfExists('password','has-error')}">
                            <label for="password-new" class="form-label">
                                ${msg("passwordNew")} <span class="required">*</span>
                            </label>
                            <div class="input-wrapper">
                                <input type="password" id="password-new" name="password-new" class="form-input with-icon"
                                       autofocus autocomplete="new-password"
                                       aria-invalid="<#if messagesPerField.existsError('password')>true</#if>"
                                       placeholder="Digite sua nova senha"
                                />
                                <button type="button" class="toggle-password" data-target="password-new" aria-label="Mostrar/ocultar senha">
                                    <svg class="eye-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"/>
                                    </svg>
                                    <svg class="eye-off-icon" style="display: none;" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.878 9.878L3 3m6.878 6.878L12 12m0 0l3.122 3.122M12 12L9 9m3 3l3-3m-3 3l-3 3"/>
                                    </svg>
                                </button>
                            </div>
                            <#if messagesPerField.existsError('password')>
                                <span class="error-message" aria-live="polite">
                                    ${kcSanitize(messagesPerField.get('password'))?no_esc}
                                </span>
                            </#if>
                        </div>

                        <!-- Confirmação da senha -->
                        <div class="form-group ${messagesPerField.printIfExists('password-confirm','has-error')}">
                            <label for="password-confirm" class="form-label">
                                ${msg("passwordConfirm")} <span class="required">*</span>
                            </label>
                            <div class="input-wrapper">
                                <input type="password" id="password-confirm" name="password-confirm" class="form-input with-icon"
                                       autocomplete="new-password"
                                       aria-invalid="<#if messagesPerField.existsError('password-confirm')>true</#if>"
                                       placeholder="Confirme sua nova senha"
                                />
                                <button type="button" class="toggle-password" data-target="password-confirm" aria-label="Mostrar/ocultar senha">
                                    <svg class="eye-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"/>
                                    </svg>
                                    <svg class="eye-off-icon" style="display: none;" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.878 9.878L3 3m6.878 6.878L12 12m0 0l3.122 3.122M12 12L9 9m3 3l3-3m-3 3l-3 3"/>
                                    </svg>
                                </button>
                            </div>
                            <#if messagesPerField.existsError('password-confirm')>
                                <span class="error-message" aria-live="polite">
                                    ${kcSanitize(messagesPerField.get('password-confirm'))?no_esc}
                                </span>
                            </#if>
                        </div>
                    </div>

                    <div class="submit-section">
                        <input class="update-password-button" type="submit" value="${msg("doSubmit")}"/>
                    </div>

                    <div class="back-to-login">
                        <a href="${url.loginRestartFlowUrl}">
                            <svg class="back-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"/>
                            </svg>
                            Voltar ao login
                        </a>
                    </div>
                </form>
            </div>
        </div>

        <script>
            document.addEventListener('DOMContentLoaded', function() {
                // Toggle para mostrar/ocultar senhas
                const toggleButtons = document.querySelectorAll('.toggle-password');
                
                toggleButtons.forEach(button => {
                    button.addEventListener('click', function(e) {
                        e.preventDefault();
                        const targetId = this.getAttribute('data-target');
                        const input = document.getElementById(targetId);
                        const eyeIcon = this.querySelector('.eye-icon');
                        const eyeOffIcon = this.querySelector('.eye-off-icon');
                        
                        if (input.type === 'password') {
                            input.type = 'text';
                            eyeIcon.style.display = 'none';
                            eyeOffIcon.style.display = 'block';
                        } else {
                            input.type = 'password';
                            eyeIcon.style.display = 'block';
                            eyeOffIcon.style.display = 'none';
                        }
                    });
                });

                // Validação em tempo real de confirmação de senha
                const passwordInput = document.getElementById('password-new');
                const confirmPasswordInput = document.getElementById('password-confirm');
                
                if (passwordInput && confirmPasswordInput) {
                    const validatePasswords = () => {
                        if (confirmPasswordInput.value !== '' && 
                            passwordInput.value !== confirmPasswordInput.value) {
                            confirmPasswordInput.classList.add('error');
                            confirmPasswordInput.setAttribute('aria-invalid', 'true');
                        } else {
                            confirmPasswordInput.classList.remove('error');
                            confirmPasswordInput.setAttribute('aria-invalid', 'false');
                        }
                    };
                    
                    confirmPasswordInput.addEventListener('input', validatePasswords);
                    passwordInput.addEventListener('input', validatePasswords);
                }

                // Indicador de força da senha
                if (passwordInput) {
                    passwordInput.addEventListener('input', function() {
                        // Aqui pode adicionar lógica para mostrar força da senha
                    });
                }
            });
        </script>
    <#elseif section = "info">
        <#-- Info section vazia para evitar duplicação -->
    </#if>
</@layout.registrationLayout>