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

Requires Redis and MongoDB (Locally or via Cloud).
Add connection strings for both into appsettings.json.
