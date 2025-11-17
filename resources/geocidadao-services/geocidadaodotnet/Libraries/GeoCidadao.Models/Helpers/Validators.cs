namespace GeoCidadao.Models.Helpers
{
    public static class Validators
    {
        public static bool ValidateIpAddress(this string ipAddress)
        {
            UriHostNameType hostName = Uri.CheckHostName(ipAddress);
            return hostName != UriHostNameType.Unknown;
        }

        public static bool IsPrivateIpAddress(string ipAddress)
        {
            if (ipAddress == "localhost" || ipAddress == "127.0.0.1" || ipAddress == "::1")
                return true;

            int[] ipParts = ipAddress
                .Split(".", StringSplitOptions.RemoveEmptyEntries)
                .Select(static s => int.Parse(s.Replace(":", "").Replace("f", ""))).ToArray();

            if (ipParts[0] == 10 ||
                (ipParts[0] == 192 && ipParts[1] == 168) ||
                (ipParts[0] == 172 && ipParts[1] >= 16 && ipParts[1] <= 31))
            {
                return true;
            }

            return false;
        }
    }
}