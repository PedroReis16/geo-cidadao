<#import "template.ftl" as layout>
<#import "passkeys.ftl" as passkeys>

<@layout.registrationLayout displayMessage=!messagesPerField.existsError('username','password')
    displayInfo=realm.password && realm.registrationAllowed && !registrationDisabled??; section>

    <#if section == "header">
        ${msg("loginAccountTitle")}

    <#elseif section == "form">
    <div class="login-container">
        <div class="logo-container">
            <img id="logo" src="" alt="Logo">
            <h1>Bem-vindo</h1>
            <p>Faça login para continuar</p>
        </div>

        <div id="kc-form">
            <div id="kc-form-wrapper">
                <#if realm.password>
                    <form id="kc-form-login"
                          onsubmit="login.disabled = true; return true;"
                          action="${url.loginAction}" method="post">

                        <#-- Campo de usuário -->
                        <#if !usernameHidden??>
                            <div class="form-group">
                                <label for="username">
                                    <#if !realm.loginWithEmailAllowed>
                                        ${msg("username")}
                                    <#elseif !realm.registrationEmailAsUsername>
                                        ${msg("usernameOrEmail")}
                                    <#else>
                                        ${msg("email")}
                                    </#if>
                                </label>
                                <input type="text" id="username" name="username"
                                       value="${(login.username!'')}"
                                       placeholder="seu@email.com"
                                       autofocus
                                       autocomplete="${(enableWebAuthnConditionalUI?has_content)?then('username webauthn', 'username')}"
                                       aria-invalid="<#if messagesPerField.existsError('username','password')>true</#if>"
                                       required>

                                <#if messagesPerField.existsError('username','password')>
                                    <span class="input-error" aria-live="polite">
                                        ${kcSanitize(messagesPerField.getFirstError('username','password'))?no_esc}
                                    </span>
                                </#if>
                            </div>
                        </#if>

                        <#-- Campo de senha -->
                        <div class="form-group">
                            <label for="password">${msg("password")}</label>
                            <input type="password" id="password" name="password"
                                   placeholder="Digite sua senha"
                                   autocomplete="current-password"
                                   aria-invalid="<#if messagesPerField.existsError('username','password')>true</#if>"
                                   required>
                        </div>

                        <#-- Opções -->
                        <div class="form-options">
                            <#if realm.rememberMe && !usernameHidden??>
                                <label class="remember-me">
                                    <input type="checkbox" id="rememberMe" name="rememberMe"
                                           <#if login.rememberMe?? && login.rememberMe>checked</#if>>
                                    <span>${msg("rememberMe")}</span>
                                </label>
                            </#if>

                            <#if realm.resetPasswordAllowed>
                                <a href="${url.loginResetCredentialsUrl}" class="forgot-password">${msg("doForgotPassword")}</a>
                            </#if>
                        </div>

                        <div id="kc-form-buttons">
                            <input type="hidden" id="id-hidden-input" name="credentialId"
                                   <#if auth.selectedCredential?has_content>value="${auth.selectedCredential}"</#if> />
                            <button type="submit" name="login" id="kc-login" class="login-button">
                                ${msg("doLogIn")}
                            </button>
                        </div>
                    </form>
                </#if>
            </div>
        </div>

        <@passkeys.conditionalUIData />

        <#-- Link de cadastro -->
        <#if realm.password && realm.registrationAllowed && !registrationDisabled??>
            <div class="signup-link">
                ${msg("noAccount")}
                <a href="${url.registrationUrl}">${msg("doRegister")}</a>
            </div>
        </#if>
    </div>

    <script type="module" src="${url.resourcesPath}/js/script.js"></script>

    <#elseif section == "socialProviders">
        <#if realm.password && social?? && social.providers?has_content>
            <div id="kc-social-providers" class="${properties.kcFormSocialAccountSectionClass!}">
                <hr/>
                <h2>${msg("identity-provider-login-label")}</h2>
                <ul class="${properties.kcFormSocialAccountListClass!}">
                    <#list social.providers as p>
                        <li>
                            <a id="social-${p.alias}"
                               class="${properties.kcFormSocialAccountListButtonClass!}"
                               href="${p.loginUrl}">
                                <#if p.iconClasses?has_content>
                                    <i class="${properties.kcCommonLogoIdP!} ${p.iconClasses!}" aria-hidden="true"></i>
                                </#if>
                                <span class="${properties.kcFormSocialAccountNameClass!}">
                                    ${p.displayName!}
                                </span>
                            </a>
                        </li>
                    </#list>
                </ul>
            </div>
        </#if>
    </#if>
</@layout.registrationLayout>
