# ChangeTracker - empower your releases

ChangeTracker is a web service that enables you to keep track of your releases and changes.  

Due to the continuous movement towards microservices, releases are harder to track.  
Often monoliths get subdivided into many micro/nano services, each of them are deployed independently with its own versioning.  

Imagine you're a product owner who is responsible for many products.  
In order to communicate the changes to the customer you need to be up-to-date about the releases.  
Maybe you talk to the developer directly or call an api info endpoint to get the latest deployed version.  
After that, you check what's inside this release in your ticket system to be informed about the latest changes.  

This can be quite cumbersome with an increasing number of products.  

A possible solution is to invert the dependencies.  
The management team where the product owner belongs to should not depend on developers,  
both should depend on a web service.  

![Dependencies](./docs/assets/ChangeTracker.png)  

The development team automatically pushes its changes during deployment with all the information the management team needs.

## Disclaimer

This is a side project and should not be used in a productive environment.  

## Architecture

The architecture of the app is mainly influenced by:

* Robert C. Martin's book Clean Architecture
* Plainionist's article series [Implementing-Clean-Architecture](http://www.plainionist.net/Implementing-Clean-Architecture-Overview/)
* [CandiedOrange's](https://softwareengineering.stackexchange.com/users/131624/candied-orange) [answers](https://softwareengineering.stackexchange.com/search?q=user:131624+[clean-architecture]) on softwareengineering.stackexchange

## Key Features

* Multi-tenant capabilities
* Role-based access control

## CI/CD

Every commit pushed to the remote repo triggers the **Continuous Integration** stage  
where the app is built and all tests are executed.  

The **Continuous Delivery** stage will run afterwards if the commit was tagged.  
By doing so the app gets deployed to the staging environment  
and after a manual approval to the production environment.  

Tag names must be a valid SemVer 2.0.0

### Overview

![CICD](./docs/assets/CI_CD.png)

## Environments

### Testing

This environment consists only of a database that is used by integration tests.

### Staging

Is a replica of the production environment.  
Uses the same Docker registry and App Service Plan as the production system.  

[Staging](https://app-change-tracker-staging.azurewebsites.net/)

### Production

Actual app

[Production](https://app-change-tracker.azurewebsites.net/)

## Database

I decided to use a SQL database rather than a NoSQL db,  
because relational databases are a mature technology that meets most of my requirements.

[Database Schema](./docs/assets/ChangeTrackerDbSchema.png)

## DbConnection

There are two rules of thumb when working with db connections

* Open it as late as possible (during an http request)
* Close it as soon as possible

### How to get a db connection?  

Two types were designed to work with db connections.  

* `Func<IDbConnection>`
* `IDbAccessor`

These two types can be injected into the constructor  
but only in the data access layer.

### `Func<IDbConnection>`

This factory should be used in situations where no transaction is required.  
An example is the authentication handler that verifies the user's identity.  
This will never be part of a transaction.  

Things to keep in mind when working with `Func<IDbConnection>`.

* Wrap the connection in a `using` block since it's an ephemeral disposable
* Does not work with `IUnitOfWork`
* Can be used in concurrent situations

### `IDbAccessor`

Contains only the property `DbConnection` that can be used safely  
without worrying about opening or closing/disposing connections or transactions.  
This is handled by the DbSession, Dapper and the DI Container.  

`IDbAccessor` was designed to deal with transactions  
that are controlled by **Unit of Works** in the application layer.  

Things to keep in mind when working with `IDbAccessor`

* Do not dispose connections
* Do not open or close connections
* Do not begin transactions
* Do not use it in concurrent scenarios
* Use it with Dapper only
