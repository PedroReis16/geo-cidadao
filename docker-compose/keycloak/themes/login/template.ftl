<#macro registrationLayout bodyClass="" displayInfo=false displayMessage=true displayRequiredFields=false>
<!DOCTYPE html>
<html lang="${locale!'en'}"

<head>
    <meta charset="utf-8">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <meta name="robots" content="noindex, nofollow">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <#if properties.meta?has_content>
        <#list properties.meta?split(' ') as meta>
            <meta name="${meta?split('==')[0]}" content="${meta?split('==')[1]}"/>
        </#list>
    </#if>
    
    <title>${msg("loginTitle",(realm.displayName!''))}</title>
    
    <link rel="icon" type="image/png" href="${url.resourcesPath}/img/favicon.png" />
    
    <#if properties.stylesCommon?has_content>
        <#list properties.stylesCommon?split(' ') as style>
            <link href="${url.resourcesCommonPath}/${style}" rel="stylesheet" />
        </#list>
    </#if>
    
    <#if properties.styles?has_content>
        <#list properties.styles?split(' ') as style>
            <link href="${url.resourcesPath}/${style}" rel="stylesheet" />
        </#list>
    </#if>
    
    <#if properties.scripts?has_content>
        <#list properties.scripts?split(' ') as script>
            <script src="${url.resourcesPath}/${script}" type="text/javascript"></script>
        </#list>
    </#if>
    
    <#if scripts??>
        <#list scripts as script>
            <script src="${script}" type="text/javascript"></script>
        </#list>
    </#if>

    <script>
        // Detecta mudanças no tema do sistema e atualiza o logo
        if (window.matchMedia) {
            const darkModeQuery = window.matchMedia('(prefers-color-scheme: dark)');
            
            // Função para atualizar o logo baseado no tema
            function updateLogo(isDark) {
                const logoImg = document.getElementById('kc-logo-img');
                if (logoImg) {
                    const logoLight = '${url.resourcesPath}/img/logo-light.png';
                    const logoDark = '${url.resourcesPath}/img/logo-dark.png';
                    logoImg.src = isDark ? logoDark : logoLight;
                }
            }
            
            // Atualiza logo quando a página carrega
            document.addEventListener('DOMContentLoaded', function() {
                updateLogo(darkModeQuery.matches);
            });
            
            // Escuta mudanças no tema do sistema em tempo real
            darkModeQuery.addEventListener('change', function(e) {
                updateLogo(e.matches);
            });
        }
    </script>
</head>

<body class="${properties.kcBodyClass!} <#if bodyClass??>${bodyClass}</#if>">
    <div id="kc-container" class="${properties.kcContainerClass!}">
        <div id="kc-container-wrapper" class="${properties.kcContainerWrapperClass!}">

            <div id="kc-content">
                <div id="kc-content-wrapper">

                    <#-- Header movido para dentro do card -->
                    <div id="kc-header" class="${properties.kcHeaderClass!}">
                        <div id="kc-header-wrapper" class="${properties.kcHeaderWrapperClass!}">
                            <#nested "header">
                        </div>
                    </div>

                    <#-- Mensagens de alerta -->
                    <#if displayMessage && message?has_content && (message.type != 'warning' || !isAppInitiatedAction??)>
                        <div class="alert alert-${message.type}">
                            <#if message.type = 'success'><span>✓</span></#if>
                            <#if message.type = 'warning'><span>⚠</span></#if>
                            <#if message.type = 'error'><span>✕</span></#if>
                            <#if message.type = 'info'><span>ℹ</span></#if>
                            <span>${kcSanitize(message.summary)?no_esc}</span>
                        </div>
                    </#if>

                    <#nested "form">

                    <#if displayInfo>
                        <div id="kc-info" class="${properties.kcSignUpClass!}">
                            <div id="kc-info-wrapper" class="${properties.kcInfoAreaWrapperClass!}">
                                <#nested "info">
                            </div>
                        </div>
                    </#if>
                </div>
            </div>

        </div>
    </div>
</body>
</html>
</#macro>