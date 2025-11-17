using GeoCidadao.GerenciamentoPostsAPI.Contracts;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs;
using GeoCidadao.GerenciamentoPostsAPI.Model.DTOs.Posts;

namespace GeoCidadao.GerenciamentoPostsAPI.Services
{
    public class LocationsService(IPostService postService) : ILocationsService
    {
        private readonly IPostService _postService = postService;

        public async Task<GeoJsonFeatureCollectionDTO> GetPostsAsGeoJsonAsync(LocationQueryDTO locationQuery)
        {
            var postsWithLocation = await _postService.GetPostsByLocationAsync(locationQuery);

            var features = postsWithLocation
                .Where(p => p.Location != null)
                .Select(p => new GeoJsonFeatureDTO
                {
                    Geometry = new GeoJsonGeometryDTO
                    {
                        Type = "Point",
                        Coordinates = new[] { p.Location!.Longitude, p.Location!.Latitude }
                    },
                    Properties = new PostMapPropertiesDTO
                    {
                        Id = p.Id,
                        Content = p.Content,
                        UserId = p.UserId,
                        CreatedAt = p.CreatedAt,
                        Address = p.Location.Address,
                        City = p.Location.City,
                        State = p.Location.State
                    }
                })
                .ToList();

            return new GeoJsonFeatureCollectionDTO
            {
                Features = features
            };
        }
    }
}