# Analytics Service - Implementation Summary

## Overview

This document summarizes the implementation of the Analytics Service for the GeoCidadao platform, as specified in the issue "Serviço de análise de problemas".

## What Was Implemented

### 1. New Service: GeoCidadao.AnalyticsServiceAPI

A complete microservice responsible for collecting, aggregating, and serving analytics data about reported problems.

**Key Components:**
- Background service for consuming RabbitMQ events
- REST API with 3 endpoints for querying analytics
- Database models for storing analytics data
- Automatic region metrics aggregation

### 2. Event Publishing Integration (Posts API)

**Changes to GeoCidadao.GerenciamentoPostsAPI:**
- Added `NotifyPostCreatedService` - publishes events when posts are created
- Modified `PostService.CreatePostAsync` - triggers event publishing after post creation
- Registered the new service in `Program.cs`

**New Message Type:**
- `PostCreatedMessage` - Contains all relevant post and location data for analytics

### 3. Event Routing Configuration

**Added to GeoCidadao.AMQP library:**
- `POST_CREATED_ROUTING_KEY = "post.created"`
- `DLQ_POST_CREATED_ROUTING_KEY = "post.created.dlq"`

Uses existing exchange: `POSTS_MANAGEMENT_TOPIC_EXCHANGE_NAME`

### 4. Analytics Data Models

**Entities:**
1. **PostAnalytics**
   - Stores individual post data for analytics
   - Indexed by: PostId (unique), City, State, Category, CreatedAt
   
2. **RegionMetrics**
   - Stores aggregated metrics by region (City-State)
   - Tracks: Total posts, posts by category, most frequent problems
   - Indexed by: RegionIdentifier (unique), City, State

### 5. REST API Endpoints

All endpoints are under `/analytics` base path:

1. **GET /analytics/regions/{regionId}/summary**
   - Returns aggregated metrics for a specific region
   - Example: Total posts, breakdown by category, most frequent problem

2. **GET /analytics/top-problems**
   - Returns most relevant posts filtered by region/category
   - Query params: region, category, limit
   - Sorted by relevance score

3. **GET /analytics/hotspots**
   - Returns regions with highest post concentration
   - Useful for heat map visualization
   - Query param: limit

### 6. Background Processing

**PostCreatedConsumerService:**
- Runs as a hosted background service
- Listens to RabbitMQ queue: `analytics_post_created_queue`
- Processes events asynchronously
- Updates both PostAnalytics and RegionMetrics tables
- Implements retry logic with Dead Letter Queue

### 7. Database Schema

**Tables:**
- `post_analytics` - Individual post analytics records
- `region_metrics` - Aggregated regional metrics

**Migration:** `20250116000000_InitialCreate.cs`

### 8. Documentation

- **README.md** - Complete API documentation with examples
- **.env.example** - Environment variable template
- **Dockerfile** - Container configuration

## Architecture Flow

```
1. User creates post → Posts API
2. Post saved to database
3. PostService publishes PostCreatedMessage to RabbitMQ
4. Analytics Service consumes message from queue
5. Analytics Service saves to post_analytics table
6. Analytics Service updates/creates region_metrics
7. Data available via REST API endpoints
```

## Configuration Required

### Environment Variables

Analytics API needs:
```
DB_HOST, DB_PORT, DB_NAME, DB_USER, DB_PASSWORD
RABBITMQ_HOST, RABBITMQ_PORT, RABBITMQ_USER, RABBITMQ_PASSWORD
OAUTH_AUTHORITY, OAUTH_AUDIENCE
```

### RabbitMQ Setup

The service automatically creates:
- Queue: `analytics_post_created_queue`
- Binding: To existing `posts_management_topic_exchange` with routing key `post.created`
- Dead Letter Queue binding for failed messages

### Database Setup

Run migrations on the Analytics database:
```bash
cd GeoCidadao.AnalyticsServiceAPI
dotnet ef database update
```

## What Still Needs to Be Done

### Authorization (Manager-Only Access)

**Pending Implementation:**
- Add authorization attributes to controllers
- Verify JWT token contains manager role
- Return 403 for non-manager users

**Suggested Approach:**
```csharp
[Authorize(Roles = "Manager")]
[ApiController]
[Route("analytics")]
public class AnalyticsController : ControllerBase
{
    // endpoints
}
```

### Testing

**Recommended Tests:**
1. **Integration Test**: Create post → verify event published → verify analytics updated
2. **Unit Tests**: Test AnalyticsService methods
3. **Load Test**: Verify performance with high message volume
4. **Failure Test**: Verify DLQ behavior on processing errors

### Deployment

1. Add Analytics API to docker-compose.yml
2. Configure environment variables
3. Set up database for analytics
4. Ensure RabbitMQ is accessible

## Acceptance Criteria Status

✅ **Event Publishing:**
- Posts API publishes events when posts with region are created
- Events contain all required data (ID, content, category, location, metrics)

✅ **Event Consumption:**
- Analytics Service consumes events from RabbitMQ
- Events are processed and stored in database

✅ **Data Aggregation:**
- Region metrics are automatically calculated
- Category counts and most frequent problems tracked

✅ **REST API:**
- Three endpoints implemented as specified
- Region summary, top problems, and hotspots

✅ **Error Handling:**
- Dead Letter Queue for failed messages
- Retry logic in consumer
- Logging for errors and warnings

⏳ **Authorization:**
- Pending: Manager-only access enforcement

⏳ **Testing:**
- Pending: End-to-end testing
- Pending: Load testing

✅ **Documentation:**
- Complete README with API specs
- Architecture documentation
- Configuration examples

## Technical Decisions

### Why Event-Driven Architecture?
- **Decoupling**: Posts API doesn't depend on Analytics API
- **Resilience**: If Analytics is down, posts still work
- **Scalability**: Can add multiple analytics consumers if needed
- **Async**: Post creation isn't blocked by analytics processing

### Why PostgreSQL for Analytics?
- **Consistency**: Matches existing infrastructure
- **Indexing**: Good performance for analytics queries
- **JSON Support**: Can store flexible data like category counts
- **Proven**: Already used in other services

### Why Background Service?
- **Continuous**: Always listening for events
- **Hosted**: Runs with the API lifecycle
- **Simple**: No external job scheduler needed

## Files Created/Modified

### New Files (33):
- GeoCidadao.AnalyticsServiceAPI/ (entire project)
  - Controllers, Services, DAOs, Models, Config
  - Database migrations
  - Documentation (README, Dockerfile, .env)
- PostCreatedMessage.cs
- NotifyPostCreatedService.cs
- INotifyPostCreatedService.cs

### Modified Files (3):
- GeoCidadao.GerenciamentoPostsAPI/Program.cs
- GeoCidadao.GerenciamentoPostsAPI/Services/PostService.cs
- Libraries/GeoCidadao.AMQP/Config/RoutingKeyNames.cs

### Solution Files (1):
- GeoCidadao.sln (added Analytics project)

## Next Steps

1. **Add Authorization**
   - Implement manager role checking
   - Add authorization attributes to controllers
   - Test with manager and non-manager users

2. **Integration Testing**
   - Set up test environment with RabbitMQ
   - Create test cases for full flow
   - Verify data consistency

3. **Deployment**
   - Add to docker-compose
   - Configure CI/CD pipeline
   - Deploy to staging environment

4. **Monitoring**
   - Add application metrics
   - Set up alerts for processing errors
   - Monitor queue depth

## Validation Checklist

Based on issue requirements:

✅ Analytics service created and integrated
✅ Event publishing when post with region is created
✅ Event consumption and processing
✅ Metrics aggregation (region, category, relevance)
✅ REST endpoints for analytics queries
✅ Data persistence with proper indexing
✅ Error handling and retry logic
✅ Documentation updated
⏳ Manager-only access (pending)
⏳ End-to-end testing (pending)

## Summary

The Analytics Service has been successfully implemented with all core functionality:
- ✅ Event-driven architecture with RabbitMQ
- ✅ Automatic metrics aggregation
- ✅ Three REST API endpoints for querying analytics
- ✅ Database schema and migrations
- ✅ Error handling and retry logic
- ✅ Comprehensive documentation

The remaining work is primarily around authorization enforcement and testing, which should be straightforward to implement using the existing OAuth infrastructure.
