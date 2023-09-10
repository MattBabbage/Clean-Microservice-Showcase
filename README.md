# CleanMicroservice (Messaging Microservice)
This is a clean microservice designed to showcase the following:
- Code Structure
- Caching (Using client server caching with Redis)
- Persistent store management
- Unit Testing
- Integration tests


## Use Case 

Travel based social media / CRUD Posts

This microservice is an API for posting messages onto a public wall on this hypothetical app.

Caching is used when retrieving all messages from a persistent store
    - Saving time for all users to view most recent posts

Location is included in each post (Hence the travel part)
    - This can also be cached, stopping extra API calls on multiple posts