using System.IdentityModel.Tokens.Jwt;

namespace GeoCidadao.Model.OAuth
{
    public class OAuthCredentials
    {
        private string _accessToken = null!;
        public string AccessToken
        {
            get => _accessToken; set
            {
                _accessToken = value;
                ExpiresAt = GetExpirationDate(value);
            }
        }
        public string RefreshToken { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }

        private DateTime GetExpirationDate(string token)
        {
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var exp = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp")?.Value;

            if (exp != null && long.TryParse(exp, out var expValue))
            {
                var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expValue);
                return expirationTime.DateTime;
            }

            throw new ArgumentException("Não foi possível obter a data de expiração do token");
        }
    }
}