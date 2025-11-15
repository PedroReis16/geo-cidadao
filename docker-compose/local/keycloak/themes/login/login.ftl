<#import "template.ftl" as layout>
<@layout.registrationLayout displayMessage=!messagesPerField.existsError('username','password') displayInfo=realm.password && realm.registrationAllowed && !registrationDisabled??; section>
    <#if section = "header">
        <div class="logo-container">
            <img id="kc-logo-img" src="${url.resourcesPath}/img/logo-light.png" alt="GeoCidadão">
            <h1 class="kc-logo-text">Bem-vindo</h1>
            <p>Faça login para continuar</p>
        </div>
    <#elseif section = "form">
    <div id="kc-form">
      <div id="kc-form-wrapper">
        <#if realm.password>
            <form id="kc-form-login" onsubmit="login.disabled = true; return true;" action="${url.loginAction}" method="post">
                <#if !usernameHidden??>
                    <div class="form-group">
                        <label for="username" class="form-label">
                            <#if !realm.loginWithEmailAllowed>
                                ${msg("username")}
                            <#elseif !realm.registrationEmailAsUsername>
                                ${msg("usernameOrEmail")}
                            <#else>
                                ${msg("email")}
                            </#if>
                        </label>

                        <input tabindex="1" id="username" class="form-input" name="username" 
                               value="${(login.username!'')}" type="text" autofocus autocomplete="off"
                               aria-invalid="<#if messagesPerField.existsError('username','password')>true</#if>"
                               placeholder="<#if !realm.loginWithEmailAllowed>${msg("username")}<#elseif !realm.registrationEmailAsUsername>${msg("usernameOrEmail")}<#else>${msg("email")}</#if>"
                        />

                        <#if messagesPerField.existsError('username','password')>
                            <span id="input-error" class="error-message" aria-live="polite">
                                ${kcSanitize(messagesPerField.getFirstError('username','password'))?no_esc}
                            </span>
                        </#if>
                    </div>
                </#if>

                <div class="form-group">
                    <label for="password" class="form-label">${msg("password")}</label>
                    <input tabindex="2" id="password" class="form-input" name="password" type="password" 
                           autocomplete="off"
                           aria-invalid="<#if messagesPerField.existsError('username','password')>true</#if>"
                           placeholder="${msg("password")}"
                    />
                </div>

                <div class="form-options">
                    <#if realm.rememberMe && !usernameHidden??>
                        <label class="remember-me">
                            <#if login.rememberMe??>
                                <input tabindex="3" id="rememberMe" name="rememberMe" type="checkbox" checked>
                            <#else>
                                <input tabindex="3" id="rememberMe" name="rememberMe" type="checkbox">
                            </#if>
                            <span>${msg("rememberMe")}</span>
                        </label>
                    <#else>
                        <div></div>
                    </#if>
                    
                    <#if realm.resetPasswordAllowed>
                        <a tabindex="5" href="${url.loginResetCredentialsUrl}" class="forgot-password">
                            ${msg("doForgotPassword")}
                        </a>
                    </#if>
                </div>

                <div id="kc-form-buttons">
                    <input type="hidden" id="id-hidden-input" name="credentialId" 
                           <#if auth.selectedCredential?has_content>value="${auth.selectedCredential}"</#if>/>
                    <button tabindex="4" class="login-button" name="login" id="kc-login" type="submit">
                        ${msg("doLogIn")}
                    </button>
                </div>

                <#if realm.password && realm.registrationAllowed && !registrationDisabled??>
                    <div class="signup-link">
                        ${msg("noAccount")}
                        <a tabindex="6" href="${url.registrationUrl}">${msg("doRegister")}</a>
                    </div>
                </#if>
            </form>
        </#if>
      </div>
    </div>
    <#elseif section = "info" >
        <#-- Info section removida para evitar duplicação do link de registro -->
    </#if>

</@layout.registrationLayout>