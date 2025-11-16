using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace GeoCidadao.AnalyticsServiceAPI.Config
{
    public class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value)
        {
            if (value == null) return null;
            return value.ToString()?.ToLowerInvariant();
        }
    }
}
