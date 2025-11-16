using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GeoCidadao.AnalyticsServiceAPI.Config
{
    public class TagDescriptionsDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags =
            [
                new OpenApiTag { Name = "Analytics", Description = "Analytics and metrics for manager users" }
            ];
        }
    }
}
