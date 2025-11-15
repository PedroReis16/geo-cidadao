<#import "template.ftl" as layout>
<@layout.registrationLayout bodyClass="register-page" displayMessage=!messagesPerField.existsError('firstName','lastName','email','username','password','password-confirm') displayInfo=realm.password && realm.registrationAllowed && !registrationDisabled??; section>
    <#if section = "header">
        <div class="logo-container">
            <img id="kc-logo-img" src="${url.resourcesPath}/img/logo-light.png" alt="GeoCidadão">
            <h1 class="kc-logo-text">Criar Conta</h1>
            <p>Preencha os dados para se cadastrar</p>
        </div>
    <#elseif section = "form">
        <div id="kc-form">
            <div id="kc-form-wrapper">
                <form id="kc-register-form" class="${properties.kcFormClass!}" action="${url.registrationAction}" method="post">
                    
                    <div class="form-grid">
                        <!-- Email -->
                        <div class="form-group full-width ${messagesPerField.printIfExists('email','has-error')}">
                            <label for="email" class="form-label">
                                ${msg("email")} <span class="required">*</span>
                            </label>
                            <input type="text" id="email" class="form-input" name="email"
                                   value="${(register.formData.email!'')}" autocomplete="email"
                                   aria-invalid="<#if messagesPerField.existsError('email')>true</#if>"
                                   placeholder="${msg("email")}"
                            />
                            <#if messagesPerField.existsError('email')>
                                <span class="error-message show" aria-live="polite">
                                    ${kcSanitize(messagesPerField.get('email'))?no_esc}
                                </span>
                            </#if>
                        </div>

                        <!-- First Name -->
                        <div class="form-group ${messagesPerField.printIfExists('firstName','has-error')}">
                            <label for="firstName" class="form-label">
                                ${msg("firstName")} <span class="required">*</span>
                            </label>
                            <input type="text" id="firstName" class="form-input" name="firstName"
                                   value="${(register.formData.firstName!'')}" autocomplete="given-name"
                                   aria-invalid="<#if messagesPerField.existsError('firstName')>true</#if>"
                                   placeholder="${msg("firstName")}"
                            />
                            <#if messagesPerField.existsError('firstName')>
                                <span class="error-message show" aria-live="polite">
                                    ${kcSanitize(messagesPerField.get('firstName'))?no_esc}
                                </span>
                            </#if>
                        </div>

                        <!-- Last Name -->
                        <div class="form-group ${messagesPerField.printIfExists('lastName','has-error')}">
                            <label for="lastName" class="form-label">
                                ${msg("lastName")} <span class="required">*</span>
                            </label>
                            <input type="text" id="lastName" class="form-input" name="lastName"
                                   value="${(register.formData.lastName!'')}" autocomplete="family-name"
                                   aria-invalid="<#if messagesPerField.existsError('lastName')>true</#if>"
                                   placeholder="${msg("lastName")}"
                            />
                            <#if messagesPerField.existsError('lastName')>
                                <span class="error-message show" aria-live="polite">
                                    ${kcSanitize(messagesPerField.get('lastName'))?no_esc}
                                </span>
                            </#if>
                        </div>

                        <!-- Username -->
                        <#if !realm.registrationEmailAsUsername>
                            <div class="form-group full-width ${messagesPerField.printIfExists('username','has-error')}">
                                <label for="username" class="form-label">
                                    ${msg("username")} <span class="required">*</span>
                                </label>
                                <input type="text" id="username" class="form-input" name="username"
                                       value="${(register.formData.username!'')}" autocomplete="username"
                                       aria-invalid="<#if messagesPerField.existsError('username')>true</#if>"
                                       placeholder="${msg("username")}"
                                />
                                <#if messagesPerField.existsError('username')>
                                    <span class="error-message show" aria-live="polite">
                                        ${kcSanitize(messagesPerField.get('username'))?no_esc}
                                    </span>
                                </#if>
                            </div>
                        </#if>

                        <!-- Password -->
                        <div class="form-group ${messagesPerField.printIfExists('password','has-error')}">
                            <label for="password" class="form-label">
                                ${msg("password")} <span class="required">*</span>
                            </label>
                            <div class="input-wrapper">
                                <input type="password" id="password" class="form-input with-icon" name="password"
                                       autocomplete="new-password"
                                       aria-invalid="<#if messagesPerField.existsError('password')>true</#if>"
                                       placeholder="••••••••"
                                />
                                <button type="button" class="toggle-password" data-target="password" aria-label="Mostrar senha">
                                    <svg class="eye-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"/>
                                    </svg>
                                    <svg class="eye-off-icon" style="display: none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21"/>
                                    </svg>
                                </button>
                            </div>
                            <#if messagesPerField.existsError('password')>
                                <span class="error-message show" aria-live="polite">
                                    ${kcSanitize(messagesPerField.get('password'))?no_esc}
                                </span>
                            </#if>
                        </div>

                        <!-- Password Confirm -->
                        <div class="form-group ${messagesPerField.printIfExists('password-confirm','has-error')}">
                            <label for="password-confirm" class="form-label">
                                ${msg("passwordConfirm")} <span class="required">*</span>
                            </label>
                            <div class="input-wrapper">
                                <input type="password" id="password-confirm" class="form-input with-icon" name="password-confirm"
                                       autocomplete="new-password"
                                       aria-invalid="<#if messagesPerField.existsError('password-confirm')>true</#if>"
                                       placeholder="••••••••"
                                />
                                <button type="button" class="toggle-password" data-target="password-confirm" aria-label="Mostrar confirmação de senha">
                                    <svg class="eye-icon" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"/>
                                    </svg>
                                    <svg class="eye-off-icon" style="display: none" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21"/>
                                    </svg>
                                </button>
                            </div>
                            <#if messagesPerField.existsError('password-confirm')>
                                <span class="error-message show" aria-live="polite">
                                    ${kcSanitize(messagesPerField.get('password-confirm'))?no_esc}
                                </span>
                            </#if>
                        </div>

                        <#if recaptchaRequired??>
                            <div class="form-group full-width">
                                <div class="g-recaptcha" data-size="compact" data-sitekey="${recaptchaSiteKey}"></div>
                            </div>
                        </#if>
                    </div>

                    <div class="submit-section">
                        <input class="register-button" type="submit" value="${msg("doRegister")}"/>
                    </div>

                    <div class="login-link">
                        <span>Já possui uma conta?</span>
                        <a href="${url.loginUrl}"> Entrar</a>
                    </div>
                </form>
            </div>
        </div>

        <script>
            // Toggle para mostrar/ocultar senhas
            document.addEventListener('DOMContentLoaded', function() {
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

                // Validação de username - apenas minúsculas, números e símbolos
                const usernameInput = document.getElementById('username');
                if (usernameInput) {
                    usernameInput.addEventListener('input', function(e) {
                        let value = e.target.value;
                        let newValue = value.replace(/[A-Z]/g, '');
                        
                        if (value !== newValue) {
                            e.target.value = newValue;
                        }
                    });
                }

                // Validação em tempo real de confirmação de senha
                const passwordInput = document.getElementById('password');
                const confirmPasswordInput = document.getElementById('password-confirm');
                
                if (passwordInput && confirmPasswordInput) {
                    const validatePasswords = () => {
                        if (confirmPasswordInput.value !== '' && 
                            passwordInput.value !== confirmPasswordInput.value) {
                            confirmPasswordInput.classList.add('error');
                        } else {
                            confirmPasswordInput.classList.remove('error');
                        }
                    };
                    
                    confirmPasswordInput.addEventListener('input', validatePasswords);
                    passwordInput.addEventListener('input', validatePasswords);
                }
            });
        </script>
    <#elseif section = "info" >
        <#-- Info section vazia para evitar duplicação -->
    </#if>

</@layout.registrationLayout>
