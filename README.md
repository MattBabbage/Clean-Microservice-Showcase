## Design Decisions:
**What is this project?**

This is a Microservice created using the Minimal API Structure.
A potential use case could be a 'travel social media company' which allows users to post a status at any location.

**Things it does include:**

- Service should maintain a cache ✅
- Service should maintain a persistent store of the looked-up values ✅
- Unit tests ⏳
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

MongoDB is used as a Database for:
- Small singular table - would never use relational features of an RDBMS
- Ease of use and setup - Emphasis is on the code not the use case