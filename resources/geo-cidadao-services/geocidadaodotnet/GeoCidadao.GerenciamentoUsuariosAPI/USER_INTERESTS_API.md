# User Interests API

## Overview

The User Interests API allows users to define and manage their interests (regions, cities, and post categories) to personalize their feed experience.

## API Endpoints

### 1. Get User Interests

**GET** `/user-interests/{userId}`

Retrieves the interests configuration for a specific user.

**Response:**
- `200 OK`: Returns user interests
- `404 Not Found`: User has no interests configured

**Example Response:**
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "region": "Centro",
  "city": "São Paulo",
  "state": "SP",
  "categories": [1, 2, 10, 20, 30]
}
```

### 2. Create User Interests

**POST** `/user-interests/{userId}`

Creates interests configuration for a user.

**Request Body:**
```json
{
  "region": "Centro",
  "city": "São Paulo",
  "state": "SP",
  "categories": [1, 2, 10, 20, 30]
}
```

**Response:**
- `201 Created`: Interests created successfully
- `400 Bad Request`: Invalid data or interests already exist

### 3. Update User Interests

**PUT** `/user-interests/{userId}`

Updates the interests configuration for a user.

**Request Body:**
```json
{
  "region": "Zona Sul",
  "city": "São Paulo",
  "state": "SP",
  "categories": [30, 31, 32, 40, 50]
}
```

**Response:**
- `200 OK`: Interests updated successfully
- `404 Not Found`: User interests not found

### 4. Delete User Interests

**DELETE** `/user-interests/{userId}`

Deletes the interests configuration for a user.

**Response:**
- `204 No Content`: Interests deleted successfully
- `404 Not Found`: User interests not found

## Post Categories

The following categories can be used in the interests configuration:

### Security (1-4)
- 1: CRIME
- 2: VANDALISM
- 3: PUBLIC_DISORDER
- 4: ILLEGAL_OCCUPATION

### Traffic and Mobility (10-15)
- 10: TRAFFIC_ACCIDENT
- 11: DAMAGED_SIGNAGE
- 12: MISSING_SIGNAGE
- 13: TRAFFIC_CONGESTION
- 14: ILLEGAL_PARKING
- 15: LACK_OF_ACCESSIBILITY

### Urban Infrastructure (20-26)
- 20: ROAD_HOLE
- 21: PUBLIC_LIGHTING
- 22: DAMAGED_PAVEMENT
- 23: CLOGGED_DRAIN
- 24: DAMAGED_BRIDGE
- 25: DAMAGED_SIDEWALK
- 26: ILLEGAL_CONSTRUCTION

### Environment (30-35)
- 30: ACCUMULATED_GARBAGE
- 31: IRREGULAR_WASTE_DISPOSAL
- 32: POLLUTION
- 33: FIRE
- 34: DEFORESTATION
- 35: FLOODING

### Public Services (40-42)
- 40: WATER_OUTAGE
- 41: POWER_OUTAGE
- 42: GARBAGE_COLLECTION_FAILURE

### Health and Well-being (50-53)
- 50: ABANDONED_ANIMAL
- 51: AGGRESSIVE_ANIMAL
- 52: LACK_OF_SANITATION
- 53: POOR_PUBLIC_SPACE_MAINTENANCE

### Social Conditions (60-62)
- 60: LACK_OF_SOCIAL_PROGRAMS
- 61: ILLEGAL_SETTLEMENT
- 62: HOMELESS_PERSON

### Events and Gatherings (70-74)
- 70: EVENT_SHOW
- 71: EVENT_PROTEST
- 72: EVENT_FAIR
- 73: TEMPORARY_INTERDICTION
- 74: LARGE_GATHERING

### Complaints (80-82)
- 80: LACK_OF_PUBLIC_RESPONSE
- 81: INEFFICIENT_INSPECTION
- 82: ABANDONED_PUBLIC_EQUIPMENT

## Data Model

### UserInterests Entity
- **id**: Unique identifier (GUID)
- **userId**: Reference to the user (GUID) - Unique
- **region**: Optional region name (max 100 chars)
- **city**: Optional city name (max 100 chars)
- **state**: Optional state abbreviation (max 50 chars)
- **categories**: Optional list of post category IDs
- **createdAt**: Creation timestamp
- **updatedAt**: Last update timestamp

## Database Migration

A database migration has been created to add the `user_interests` table:
- Migration file: `20251115205515_AddUserInterests.cs`
- Table name: `user_interests`
- Foreign key relationship with `user_profile` table (cascade delete)

## Usage for Feed Filtering

The user interests can be used by the Posts Feed service to:
1. Filter posts by region, city, or state
2. Prioritize posts from categories the user is interested in
3. Provide a personalized feed experience

When a user has no interests configured, the feed service should return a default feed without filtering.
