## Example Minimal API:
**What is this project?**

This is a Microservice created using the Minimal API Structure.

It is a CRUD application for a 'travel social media company' to create posts/statuses publically with a location attached.

It Creates posts and attatches a location on creation based on IP. It also provides the other standard functionality which is expected of CRUD APIs.

**Things it does include:**

- Service should maintain a cache ✅
- Service should maintain a persistent store of the looked-up values ✅
- Unit tests ✅
- Integration tests ✅

**Extra things I have added:** 

- Documentation/Comments via Swagger ✅ (Wanted to learn how to do this on a minimal API)
- Basic Rate Limiting ✅ (Core feature of most APIs)

**Does not include:**

- Authentication ❌ - Avoid overcomplication/Not in spec
- Authorization ❌ - Avoid overcomplication/Not in spec
- IP from HTTP Context ❌ - More difficult to demonstrate and debug.


## Design Decisions:

**Project Structure** 🧱

I have avoided using the traditional MVC setup in favour of minimal architecture, this is more fitting for a 'Microservice'.

Given that the project showcases some advanced API features, it is arguably at the limit of what a minimal structure can take before becoming messy (e.g. Parsing 5 parameters into a function whereas MVC could have dependency injected to a controller, perhaps neater).  

**Caching** 💾

Chose to use Redis in an external store (Distributed Caching). This allows the service to be fully scalable in the future, and means the cache is resilient to service downtime.

**Database / Persistent Store** 📦

MongoDB is used as a Database. This was chosen because:
- Small singular table - would never use relational features of an RDBMS
- Ease of use and setup - Emphasis is on the code not the use case


## How to run:

Git clone this repo

**Unit testing** 🧑‍🔬

Should run out of the box. 

cd into PostingService.UnitTests and run 'dotnet test'

**Live / Integration Testing** ⚡

Create an free API Key here:
https://www.abstractapi.com/api/ip-geolocation-api

```
dotnet user-secrets set "Location:ApiKey" "{Your API Key}"
```

Either host Redis/MongoDB Externally or if using docker desktop:

```
//Create redis cache
docker run -d -p 6379:6379 --name posting-redis redis:latest
//Create mongodb database
docker run -d -p 27017:27017 --name posting-mongo mongo:latest
```

*Note: If using different ports or hosting externally, change appsettings.json to reflect this*