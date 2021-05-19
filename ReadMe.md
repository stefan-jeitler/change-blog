# ChangeTracker - empower your releases

ChangeTracker is web service that enables you to keep track of your releases and changes.  

Due to the continuous movement towards microservices releases are harder to track.  
Often monoliths gets subdivided into many micro/nano services, each of them are deployed independently with its own versioning.  

Imagine you're a product owner who is responsible for many products.  
In order to communicate the changes to the customer you need to be up-to-date about the releases.  
Maybe you talk to the developer directly or call an api info endpoint to get the latest deployed version.  
After that, you check what's inside this release in your ticket system to be informed about the latest changes.  

This can be quite cumbersome with an increasing number of products.  

A possible solution is to invert the dependencies.  
The management team where the product owner belongs to should not depend on developers,  
both should depend on a web service.  

![Dependencies](./docs/assets/ChangeTracker.png)  

The development team automatically pushes its changes during deployment with all the information that the management team needs.

## Disclaimer

This is a side project and should not be used in a productive environment.  

## Architecture

The architecture of the app is mainly influenced by:

* Robert C. Martin's book Clean Architecture
* Plainionist's article series [Implementing-Clean-Architecture](http://www.plainionist.net/Implementing-Clean-Architecture-Overview/)
* [CandiedOrange's](https://softwareengineering.stackexchange.com/users/131624/candied-orange) [answers](https://softwareengineering.stackexchange.com/search?q=user:131624+[clean-architecture]) on softwareengineering.stackexchange

## Key Features

* Multi-tenant capabilities
* Role-based access control**

## CI/CD

### Continuous Integration

Every push to the remote repo triggers the `Continuous Integration` stage.  
This stage builds the app and execute all tests including the integration tests against the testing db.  

### Continuous Delivery

A deployment is triggered by a git tag.  
The message must consists of a valid SemVer 2.0.  
This will build the docker image and pushes it to the registry.  
The DbUpdater will also built and uploaded as artifact.  

![CICD](./docs/assets/CI_CD.png)

Maybe I move to `Continuous Deployment` in the future.

## Deployment Environments

### Testing

This environment consists of a database only that is used by integration tests.  

### Staging

Is a replica of the production environment.  
Uses the same Docker registry and App Service Plan as the production system.  

[Staging](https://app-change-tracker-staging.azurewebsites.net/)

### Production

Actual app

[Production](https://app-change-tracker.azurewebsites.net/)

## Database

I decided to use an SQL database rather than a NoSQL db,  
because relational databases are a mature technology that meets most of my requirements.

[Database Schema](./docs/assets/ChangeTrackerDbSchema.png)

## DbConnection

There are two rules of thumb concerning db connections

* Open it as late as possible
* Keep it open as short as possible

How to access a db connection?

Two types are designed to work with db connections.

### `Func<IDbConnection>`

### `IDbAccessor`
