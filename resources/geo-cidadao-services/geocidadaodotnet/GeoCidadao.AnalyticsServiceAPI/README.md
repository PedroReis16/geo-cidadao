# GeoCidadao Analytics Service API

## Overview

The Analytics Service is responsible for collecting, aggregating, and providing insights from posts created in the GeoCidadao platform. It enables manager users (mayors, council members, etc.) to quickly identify problem areas, trending issues, and prioritize actions based on data-driven metrics.

## Architecture

### Event-Driven Design

The Analytics Service uses an event-driven architecture to process post creation events:

```
Post Created (GeoCidadao.GerenciamentoPostsAPI)
    ↓
PostCreatedMessage published to RabbitMQ
    ↓
Analytics Service consumes event
    ↓
Data stored in Analytics Database
    ↓
Metrics aggregated by region
    ↓
Available via REST API endpoints
```

### Components

1. **Event Publisher** (in Posts API)
   - `NotifyPostCreatedService`: Publishes post creation events to RabbitMQ
   - Triggered automatically when a post with location is created

2. **Event Consumer** (in Analytics API)
   - `PostCreatedConsumerService`: Background service that listens for post creation events
   - Processes events and updates analytics data
   - Includes retry logic for failed messages

3. **Data Models**
   - `PostAnalytics`: Individual post data for analytics
   - `RegionMetrics`: Aggregated metrics by region (city-state combination)

4. **REST API**
   - Three endpoints for querying analytics data
   - Designed for manager dashboards and reporting

## API Endpoints

### 1. Get Region Summary

```http
GET /analytics/regions/{regionId}/summary
```

Returns aggregated metrics for a specific region.

**Parameters:**
- `regionId` (path): Region identifier in format "City-State" (e.g., "SaoPaulo-SP")

**Response:**
```json
{
  "regionIdentifier": "SaoPaulo-SP",
  "city": "Sao Paulo",
  "state": "SP",
  "country": "Brazil",
  "totalPosts": 150,
  "postsByCategory": {
    "ROAD_HOLE": 45,
    "PUBLIC_LIGHTING": 30,
    "ACCUMULATED_GARBAGE": 25,
    ...
  },
  "lastUpdated": "2025-01-16T10:30:00Z",
  "mostFrequentCategory": "ROAD_HOLE",
  "mostFrequentCategoryCount": 45
}
```

### 2. Get Top Problems

```http
GET /analytics/top-problems?region={region}&category={category}&limit={limit}
```

Returns the most relevant posts (problems) based on engagement metrics.

**Query Parameters:**
- `region` (optional): Filter by region identifier (format: "City-State")
- `period` (optional): Reserved for future use (time period filter)
- `category` (optional): Filter by problem category
- `limit` (optional): Maximum results (default: 10)

**Response:**
```json
[
  {
    "postId": "123e4567-e89b-12d3-a456-426614174000",
    "content": "Large pothole on Main Street...",
    "category": "ROAD_HOLE",
    "city": "Sao Paulo",
    "state": "SP",
    "latitude": -23.5505,
    "longitude": -46.6333,
    "likesCount": 45,
    "commentsCount": 12,
    "relevanceScore": 87.5,
    "createdAt": "2025-01-15T14:30:00Z"
  },
  ...
]
```

### 3. Get Hotspots

```http
GET /analytics/hotspots?limit={limit}
```

Returns regions with the highest concentration of posts (for heat mapping).

**Query Parameters:**
- `limit` (optional): Maximum results (default: 20)

**Response:**
```json
[
  {
    "regionIdentifier": "SaoPaulo-SP",
    "city": "Sao Paulo",
    "state": "SP",
    "latitude": null,
    "longitude": null,
    "postCount": 150,
    "heatScore": 1.0
  },
  {
    "regionIdentifier": "RioDeJaneiro-RJ",
    "city": "Rio de Janeiro",
    "state": "RJ",
    "latitude": null,
    "longitude": null,
    "postCount": 120,
    "heatScore": 0.8
  },
  ...
]
```

## Event Flow

### Post Creation Event

When a post is created with location information:

1. **Posts API** creates the post and location in the database
2. **Posts API** publishes `PostCreatedMessage` to RabbitMQ exchange `posts_management_topic_exchange` with routing key `post.created`
3. **Analytics API** receives the message in the `analytics_post_created_queue`
4. **Analytics API** processes the message:
   - Saves `PostAnalytics` record
   - Updates or creates `RegionMetrics` for the post's region
   - Aggregates category counts and identifies most frequent problems
5. If processing fails, message is sent to Dead Letter Queue for manual review

### Message Format

```csharp
public class PostCreatedMessage
{
    public Guid PostId { get; set; }
    public string Content { get; set; }
    public PostCategory Category { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Location information
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    
    // Engagement metrics
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public double RelevanceScore { get; set; }
}
```

## Database Schema

### post_analytics Table

Stores individual post data for analytics purposes.

| Column | Type | Description |
|--------|------|-------------|
| Id | UUID | Primary key |
| PostId | UUID | Reference to original post (unique) |
| Content | TEXT | Post content |
| Category | INT | Problem category |
| UserId | UUID | User who created the post |
| Latitude | DOUBLE | Location latitude |
| Longitude | DOUBLE | Location longitude |
| City | TEXT | City name |
| State | TEXT | State code |
| Country | TEXT | Country name |
| LikesCount | INT | Number of likes |
| CommentsCount | INT | Number of comments |
| RelevanceScore | DOUBLE | Calculated relevance |
| CreatedAt | TIMESTAMP | When post was created |
| UpdatedAt | TIMESTAMP | Last update time |

### region_metrics Table

Stores aggregated metrics by region.

| Column | Type | Description |
|--------|------|-------------|
| Id | UUID | Primary key |
| RegionIdentifier | TEXT | Unique region ID (City-State) |
| City | TEXT | City name |
| State | TEXT | State code |
| Country | TEXT | Country name |
| TotalPosts | INT | Total posts in region |
| PostsByCategory | JSON | Count per category |
| LastUpdated | TIMESTAMP | Last update time |
| MostFrequentCategory | INT | Most common problem |
| MostFrequentCategoryCount | INT | Count of most common problem |
| CreatedAt | TIMESTAMP | When record was created |
| UpdatedAt | TIMESTAMP | Last update time |

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=${DB_HOST};Port=${DB_PORT};Database=${DB_NAME};Username=${DB_USER};Password=${DB_PASSWORD}"
  },
  "RabbitMQ": {
    "HostName": "${RABBITMQ_HOST}",
    "Port": "${RABBITMQ_PORT}",
    "UserName": "${RABBITMQ_USER}",
    "Password": "${RABBITMQ_PASSWORD}"
  },
  "OAuth": {
    "Authority": "${OAUTH_AUTHORITY}",
    "Audience": "${OAUTH_AUDIENCE}",
    "RequireHttpsMetadata": false
  }
}
```

## Error Handling

### Retry Strategy

Failed message processing follows this pattern:

1. Message processing fails
2. Message is rejected (NACK) with requeue=true
3. RabbitMQ redelivers the message
4. After max retries, message is moved to Dead Letter Queue
5. Dead Letter Queue: `posts_management_topic_exchange_dlq` with routing key `post.created.dlq`

### Monitoring

Check these logs for issues:
- Post creation event publishing errors
- Message consumption errors
- Database write failures
- Invalid region data warnings

## Authorization

**Note:** Authorization for manager-only access is pending implementation.

Planned approach:
- Use JWT token validation
- Check for manager role claim
- Return 403 Forbidden for non-manager users

## Future Enhancements

1. **Time-based filtering**: Support `period` parameter for temporal analysis
2. **Trend detection**: Identify increasing/decreasing problem trends
3. **Geographic clustering**: Group nearby locations for better hotspot detection
4. **Real-time dashboards**: WebSocket support for live updates
5. **Export capabilities**: CSV/Excel export for reports
6. **Predictive analytics**: ML models to predict problem areas

## Development

### Running Migrations

```bash
cd GeoCidadao.AnalyticsServiceAPI
dotnet ef database update
```

### Running the Service

```bash
dotnet run --project GeoCidadao.AnalyticsServiceAPI
```

The API will be available at:
- Swagger UI: `http://localhost:5000/analytics-api/swagger`
- API Base: `http://localhost:5000/analytics-api`

### Testing the Flow

1. Create a post with location via Posts API
2. Check RabbitMQ management UI for message in queue
3. Verify Analytics API logs show message consumption
4. Query analytics endpoints to see the data

## Dependencies

- **.NET 8.0**
- **PostgreSQL** (analytics database)
- **RabbitMQ** (message queue)
- **Entity Framework Core** (ORM)
- **Keycloak** (OAuth provider)

## Related Services

- **GeoCidadao.GerenciamentoPostsAPI**: Source of post creation events
- **GeoCidadao.GerenciamentoUsuariosAPI**: User management and authentication
