package br.com.geocidadao.gerenciamento_usuarios_api.config;

import java.io.IOException;
import java.util.List;

import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.authority.SimpleGrantedAuthority;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Component;
import org.springframework.web.filter.OncePerRequestFilter;

import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

@Component
public class AuthAuthenticationFilter extends OncePerRequestFilter {
    private static final String ADMIN_USER = "";

    @Override
    protected void doFilterInternal(HttpServletRequest request,
            HttpServletResponse response,
            FilterChain filterChain)
            throws ServletException, IOException {

        String authHeader = request.getHeader("Authorization");

        if (authHeader != null && authHeader.equals("Bearer " + ADMIN_USER)) {
            // Cria um usuário "fake" com role ADMIN
            var authentication = new UsernamePasswordAuthenticationToken(
                    "master-user",
                    null,
                    List.of(new SimpleGrantedAuthority("ROLE_admin")));
            SecurityContextHolder.getContext().setAuthentication(authentication);
        }

        filterChain.doFilter(request, response);
    }
}
