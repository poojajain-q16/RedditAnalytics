# RedditStatsPrivate

This is a sample app to poll Reddit posts from Reddit API and build aggregates, considering the Rate Limiting applied by Reddit API. 
At this point following aggregates are calculated:

- Top 5 posts with Most Upvotes.
- Top 5 users with Most Posts.

## Setup
- .Net Core 6.0
- Replace ClientId & ClientSecret in AppConfig/appsettings.json
- Run the application.
- Hit GET /api/v1/reddit/statistics to fetch the aggregates.

## Architecture Diagram:

![RedditStatistics](https://github.com/poojajain-q16/RedditStatsPrivate/assets/138339043/d40eee2f-fb4b-4425-ae00-339c95364d3f)


## Design Approach
Pub-Sub design pattern has been used for implementation. In the current design, a simple InMemory Event Bus & InMemory Model is used
to replicate the behaviour of full a fledged event Bus and a Data Store.

### App

A RESTful API endpoint which returns the current "Top 5 posts with Most Upvotes" and "Top 5 users with Most Posts". 
This service interacts with an InMemory RedditStatistics model which can be extended as an Model mapped to a DataStore Table/Object.

### Consumer

This component polls the Event Bus continuously to fetch and process the latest Posts and stores the updated aggregates 
in InMemory RedditStatistics Model.

### Publisher

Polls Reddit API's with support for Auth and Rate limiting and publishes posts to the Event Bus.

### EventBus

- InMemoryEventBus: This is a simplistic event Bus which queues the messages as they are published and then consumers can pull the
messages from the queue at their will.

### Models

- RedditStatistics: InMemory Data Store for Statistics.
- RedditResponseMessage: API Response Body.

### Sidecar

- RateLimitingService: This service is a simplistic implementation of a Sidecar app(which ideally should be deployed as a separate service),
this service is responsible for reading the rate limiting headers and applying the delay logic to ensure we do not hit the limits.
This service also implements Circuit Breaker Pattern, to handle cases where if for any reason(errorneous implemetation or changes in Reddit.
Rate Limiting algo), reddit responds with a HTTP 429 status code(Too Many Requests), we do not allow futher requests for a specific period.

### Authentication

- RedditAuthenticationService: This service takes care of fetching and storing the access_token and checking for it's validity.
