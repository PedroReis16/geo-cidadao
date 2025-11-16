using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GeoCidadao.FeedServiceAPI.Config
{
    public class TagDescriptionsDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Tags = 
            [
                new OpenApiTag
                {
                    Name = "Feed",
                    Description = "Endpoints para gerenciamento do feed de usuários. Agrega dados de posts e usuários com suporte a cache para performance otimizada."
                },
                new OpenApiTag
                {
                    Name = "HealthCheck",
                    Description = "Endpoint para verificação de saúde da API"
                }
            ];
        }
    }
}
