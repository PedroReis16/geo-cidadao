# Search Service Implementation Summary

## Overview
This document describes the implementation of the search service (GeoCidadao.SearchServiceAPI) for the GeoCidadao platform using Elasticsearch as the search engine.

## Architecture

### Technology Stack
- **Search Engine**: Elasticsearch 8.11.0
- **Client Library**: Elastic.Clients.Elasticsearch 8.11.0
- **Message Queue**: RabbitMQ (for event-driven indexing)
- **Database**: PostgreSQL (source of truth for data)
- **Framework**: .NET 8.0

### Components

#### 1. Elasticsearch Configuration
- **Container**: Elasticsearch 8.11.0 running in Docker
- **Indices**: 
  - `geocidadao-posts`: Post documents
  - `geocidadao-users`: User documents
- **Security**: Disabled for development (should be enabled in production)
- **Memory**: 512MB heap (configurable via ES_JAVA_OPTS)

#### 2. Document Models

**PostDocument**
- `Id`: Unique identifier
- `AuthorId`, `AuthorName`: Post author information
- `Content`: Main text content
- `Category`: Post category enum
- `Tags`: List of tags
- `LocationCity`, `LocationNeighborhood`: Location text
- `LocationLatitude`, `LocationLongitude`: Geo coordinates
- `LikesCount`, `CommentsCount`: Engagement metrics
- `RelevanceScore`: Calculated relevance
- `CreatedAt`, `UpdatedAt`: Timestamps
- `IsDeleted`, `IsPublic`: Status flags

**UserDocument**
- `Id`: Unique identifier
- `Username`, `Email`: User credentials
- `FirstName`, `LastName`, `FullName`: Name components
- `CreatedAt`, `UpdatedAt`: Timestamps
- `IsDeleted`, `IsActive`: Status flags

#### 3. Services

**ElasticsearchIndexService**
- Manages index creation and configuration
- Performs CRUD operations on documents
- Handles bulk indexing for efficiency
- Automatic index initialization on startup

**SearchService**
- Implements full-text search with fuzzy matching
- Supports multi-field queries
- Handles filtering (location, author, category, dates)
- Implements relevance-based ranking
- Provides pagination support

**PostDataService & UserDataService**
- Fetch data from PostgreSQL database
- Map database entities to Elasticsearch documents
- Used by background services for reindexing

#### 4. Background Services

**PostChangedSubscriberService**
- Subscribes to: `posts_management_topic_exchange` with routing key `post.changed`
- Updates or deletes posts in the index when changed
- Handles failures with Dead Letter Queue

**UserChangedSubscriberService**
- Subscribes to: `user_management_topic_exchange` with routing key `user.changed`
- Updates or deletes users in the index when changed
- Handles failures with Dead Letter Queue

**NewUserSubscriberService**
- Subscribes to: `keycloak_events_topic_exchange` with routing key `new.user`
- Indexes new users immediately upon registration
- Handles failures with Dead Letter Queue

**ReindexingBackgroundService**
- Runs periodic full reindexing (default: every 24 hours)
- First run delayed by 5 minutes after startup
- Reindexes all posts and users to maintain consistency
- Interval configurable via `ReindexIntervalHours` setting

#### 5. API Endpoints

**Search**
- `GET /api/search?q={query}&location={location}&authorId={id}&category={category}&dateFrom={date}&dateTo={date}&page={page}&pageSize={size}&searchType={posts|users}`
  - Full-text search with filtering
  - Returns paginated results with metadata

**Index Management**
- `POST /api/search/index/post/{id}` - Index a post
- `PUT /api/search/index/post/{id}` - Update a post
- `DELETE /api/search/index/post/{id}` - Delete a post
- `POST /api/search/index/user/{id}` - Index a user

**Bulk Reindexing**
- `POST /api/search/reindex/posts` - Reindex all posts
- `POST /api/search/reindex/users` - Reindex all users

## Search Features

### Text Search
- Full-text search across content, author names, and tags
- Fuzzy matching for typo tolerance
- Boost factors for relevance:
  - Content: 2.0x
  - Tags: 1.8x  
  - Author name: 1.5x

### Filtering
- **Location**: Match by city or neighborhood
- **Author**: Filter by author ID
- **Category**: Filter by post category
- **Date Range**: Filter by creation date

### Ranking
Results are ordered by:
1. Text relevance score
2. Engagement metrics (likes, comments)
3. Recency (creation date)

### Response Format
```json
{
  "results": [...],
  "totalResults": 100,
  "page": 1,
  "pageSize": 20,
  "totalPages": 5,
  "searchTimeMs": 45
}
```

## Event-Driven Indexing

### Event Flow
1. Post/User is created/updated/deleted in the platform
2. Originating service publishes event to RabbitMQ
3. Search service consumes event
4. Document is indexed/updated/deleted in Elasticsearch
5. Changes are immediately searchable

### Event Types
- **PostChangedMessage**: Post CRUD operations
- **UserChangedMessage**: User updates
- **NewUserMessage**: New user registration

### Reliability
- Messages are acknowledged only after successful processing
- Failed messages go to Dead Letter Queue (DLQ)
- Delivery limit: 3 attempts
- Quorum queues for durability

## Configuration

### Environment Variables (docker-compose)
```env
Elasticsearch__Url=http://elasticsearch:9200
Elasticsearch__DefaultIndex=geocidadao
ExternalServices__PostsApiUrl=http://gerenciamento-posts:8080/gerenciamento-posts
ExternalServices__UsersApiUrl=http://gerenciamento-usuarios:8080/gerenciamento-usuarios
ExternalServices__TimeoutSeconds=30
ReindexIntervalHours=24
```

### Application Settings
- Elasticsearch URL and index prefix
- RabbitMQ connection details
- Database connection strings
- Reindex interval configuration
- OAuth/JWT configuration
- Logging configuration

## Deployment

### Docker Compose Services
1. `elasticsearch` - Search engine
2. `search-service` - API service
3. Dependencies: PostgreSQL, RabbitMQ

### Service Dependencies
- Requires Elasticsearch to be healthy
- Requires RabbitMQ for event consumption
- Requires PostgreSQL for data fetching

### Startup Sequence
1. Elasticsearch starts and becomes healthy
2. Search service starts
3. Elasticsearch indices are created automatically
4. Background services start consuming events
5. Reindexing service waits 5 minutes then runs first reindex

## Performance Considerations

### Indexing Performance
- Bulk indexing used for batch operations
- Individual document updates for real-time changes
- Configurable batch sizes

### Search Performance
- Elasticsearch provides sub-second search times
- Pagination prevents large result sets
- Indices optimized with appropriate field types

### Scalability
- Elasticsearch can be clustered for high availability
- Search service can be horizontally scaled
- RabbitMQ consumers can be parallelized

## Monitoring and Logging

### Logging
- Structured logging with Serilog
- MongoDB log storage
- Log levels: Info, Warning, Error
- Contextual information in all log entries

### Metrics to Monitor
- Search query latency
- Index update latency
- RabbitMQ message processing rate
- Failed indexing operations
- Elasticsearch cluster health

## Security

### Current Implementation
- Elasticsearch security disabled (development)
- JWT authentication on API endpoints
- OAuth integration for user context

### Production Recommendations
1. Enable Elasticsearch security (TLS, authentication)
2. Restrict network access to Elasticsearch
3. Implement API rate limiting
4. Add authorization checks for reindex endpoints
5. Encrypt sensitive data in documents
6. Regular security updates

## Future Enhancements

### Potential Improvements
1. **Advanced Search Features**
   - Autocomplete/suggestions
   - Spell checking
   - Faceted search
   - Geo-spatial search with radius

2. **Performance Optimization**
   - Search result caching
   - Index optimization strategies
   - Custom analyzers for Portuguese

3. **Analytics**
   - Search query analytics
   - Popular search terms
   - Click-through rate tracking

4. **Additional Indices**
   - Comments
   - Locations/regions
   - Categories/tags

5. **Monitoring**
   - Elasticsearch metrics dashboard
   - Alert system for failed indexing
   - Performance monitoring

## Testing Recommendations

### Unit Tests
- Service layer logic
- Document mapping
- Query building

### Integration Tests  
- Elasticsearch operations
- RabbitMQ event consumption
- API endpoints

### End-to-End Tests
- Full search workflow
- Index synchronization
- Event-driven updates

## Maintenance

### Regular Tasks
1. Monitor Elasticsearch disk usage
2. Review and optimize indices
3. Check RabbitMQ DLQ for failed messages
4. Review search query performance
5. Update Elasticsearch version as needed

### Troubleshooting
- Check Elasticsearch logs for errors
- Verify RabbitMQ connections
- Confirm index mappings are correct
- Test manual reindexing if data appears stale

## Conclusion

The search service provides a robust, scalable solution for searching posts and users in the GeoCidadao platform. It leverages Elasticsearch for powerful full-text search capabilities while maintaining real-time index updates through event-driven architecture.

The implementation follows best practices for:
- Separation of concerns
- Error handling
- Logging and monitoring
- Configuration management
- Security (with room for production hardening)

All code has been successfully built and passes security scanning with zero vulnerabilities detected.
