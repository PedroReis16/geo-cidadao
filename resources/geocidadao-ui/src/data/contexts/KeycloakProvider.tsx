import { ReactKeycloakProvider } from "@react-keycloak/web";
import keycloak from "../../config/keycloak";

interface Props{
    children: React.ReactNode;
}
 
const AppKeycloakProvider: React.FC<Props> = ({children}) => {
    return <ReactKeycloakProvider
        authClient={keycloak}
        initOptions={{ onLoad: "check-sso", checkLoginIframe: false }}
    >
        {children}
    </ReactKeycloakProvider>
}

export default AppKeycloakProvider;