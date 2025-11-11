<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=!messagesPerField.existsError('username') displayInfo=realm.password && realm.registrationAllowed && !registrationDisabled?? bodyClass="reset-password-page"; section>
    <#if section = "header">
        <div class="logo-container">
            <img id="kc-logo-img" src="${url.resourcesPath}/img/logo-light.png" alt="GeoCidadão">
            <h1 class="kc-logo-text">Esqueci minha senha</h1>
            <p>Digite seu email para receber instruções de redefinição</p>
        </div>
    <#elseif section = "form">
        <div id="kc-form">
            <div id="kc-form-wrapper">
                <form id="kc-reset-password-form" class="${properties.kcFormClass!}" action="${url.loginAction}" method="post">
                    
                    <div class="reset-info-box">
                        <div class="reset-icon">
                            <svg fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 7a2 2 0 012 2m4 0a6 6 0 01-7.743 5.743L11 17H9v2H7v2H4a1 1 0 01-1-1v-3a1 1 0 011-1h2.586l6.414-6.414a6 6 0 119 0z"/>
                            </svg>
                        </div>
                        <div class="reset-info-content">
                            <p class="reset-info-title">Como funciona?</p>
                            <p class="reset-info-text">
                                Enviaremos um email com um link seguro para você redefinir sua senha.
                            </p>
                        </div>
                    </div>

                    <div class="form-group ${messagesPerField.printIfExists('username','has-error')}">
                        <label for="username" class="form-label">
                            <#if !realm.loginWithEmailAllowed>
                                ${msg("username")} <span class="required">*</span>
                            <#elseif !realm.registrationEmailAsUsername>
                                ${msg("usernameOrEmail")} <span class="required">*</span>
                            <#else>
                                ${msg("email")} <span class="required">*</span>
                            </#if>
                        </label>
                        
                        <input type="text" id="username" name="username" class="form-input" 
                               autofocus value="${(auth.attemptedUsername!'')}" 
                               aria-invalid="<#if messagesPerField.existsError('username')>true</#if>"
                               placeholder="<#if !realm.loginWithEmailAllowed>${msg("username")}<#elseif !realm.registrationEmailAsUsername>${msg("usernameOrEmail")}<#else>${msg("email")}</#if>"
                        />
                        
                        <#if messagesPerField.existsError('username')>
                            <span class="error-message" aria-live="polite">
                                ${kcSanitize(messagesPerField.get('username'))?no_esc}
                            </span>
                        </#if>
                    </div>

                    <div class="submit-section">
                        <input class="reset-button" type="submit" value="${msg("doSubmit")}"/>
                    </div>

                    <div class="back-to-login">
                        <span>Lembrou sua senha?</span>
                        <a href="${url.loginRestartFlowUrl}"> Voltar ao login</a>
                    </div>
                </form>
            </div>
        </div>
    <#elseif section = "info">
        <#-- Info section vazia para evitar duplicação -->
    </#if>
</@layout.registrationLayout>