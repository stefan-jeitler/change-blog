# ChangeTracker - enhance your releases

ChangeTracker is a web service that enables you to keep track of your releases and changes.  

Due to the continuous movement towards microservices, releases are harder to track.  
Often monoliths get subdivided into many micro/nano services, each of them is deployed independently with its own versioning.  

Imagine you're a product owner who is responsible for many products.  
In order to communicate changes to the customer you need to be up-to-date about releases.  
Maybe you talk to the developer directly or call an api info endpoint to get the latest deployed version.  
After that, you check what's inside this release in your ticket system to be informed about the latest changes.  

This can be quite cumbersome with an increasing number of products.  

A possible solution is to invert the dependencies.  
The management team should not depend on developers,  
both should depend on a web service.  

![Dependencies](./docs/assets/ChangeTracker.png)  

The development team automatically pushes its changes during deployment with all the information the management team needs.

## Disclaimer

This is a side project that should not be used in productive environment.  
The basic idea for this project comes from [Keep a changelog](https://keepachangelog.com/en/1.1.0/).  

## Basic Concept

### Account

An Account is a grouping of users and prodcuts.  
User roles can be granted on account and product level.  
Product roles have precedence over account roles.  

e.g.  
A user can have a developer role on account level and a support role for one specific product within the account.  
In this case the user has for all account products developer permissions expect the one specific product.  
This is restricted to support permissions.  

This works the other way around as well.  
You can have a support role on account level and a developer role for specific products.  
So the user has more permissions for the specific products.  

However, there is not yet a user interface to do this. (future extension)

Available roles:

* DefaultUser
* Support
* Developer
* ScrumMaster
* ProductOwner
* ProductManager
* PlatformManager

### User

Currently, the following user information are stored

* Email
* Firstname
* Lastname
* Timezone (olson id)

### Product

A product is the software on which developers work.  
It has no end date, but it can be closed.  
By closing a product you stop working on that software.  
The product will still exist but it becomes read-only with all its versions and change logs.

### Version

A version is a unique string assigned to a product.  
There are three states that a version can have.  

* Not Released and Not Deleted
* Released
* Deleted

The last two make versions read-only.  
Version properties including the change logs can be modified as long it hasn't been deleted or released.  
Deleted versions can still be fetched from the api.  
The appropriate endpoints provide a switch **IncludeDeleted** to include these versions in the response.

### Pending ChangeLogs

When developing software, you often don't know to which version your changes belong.  
In order to delay this decision, you can add pending change logs.  
The change logs belong to the product only and can be moved to a version later.  
The max number of pending change logs is 100.

### ChangeLogs

Change logs are always assigned to a version  
and can be modified as long as the related version isn't released or deleted.  
The max number of change logs is 100.

### Overview

![Concept](./docs/assets/ChangeTrackerConcept.png)

## Architecture

The architecture of the app is strongly influenced by

* Robert C. Martin's book Clean Architecture
* Mark Seemann's book Dependency Injection Principles, Practices and Patterns
* Plainionist's article series [Implementing-Clean-Architecture](http://www.plainionist.net/Implementing-Clean-Architecture-Overview/)
* candied_orange's [answers](https://softwareengineering.stackexchange.com/search?q=user:131624+[clean-architecture]) on [softwareengineering.stackexchange](https://softwareengineering.stackexchange.com/)

## Key Features

* Role-based access control
* Multitenancy (single database, single schema)
* Full-text Search for versions and change logs

## CI/CD

Every commit pushed to the remote repo triggers the **Continuous Integration** stage  
where the app is built and all tests are executed.  

The **Continuous Delivery** stage will run afterwards if the commit was tagged.  
By doing so, the app gets deployed to the staging environment  
and after a manual approval to the production environment.  

Tag names must be a valid SemVer 2.0.0.  
The version in `latest-changes.json` will be compared to the tag name while deploying.  
If these values are different, the pipeline fails.  

### CiCd Overview

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

**Demo User**  
ApiKey: dEmOkEy!  
Assigned Roles:

* DefaultUser
* Developer

## Database

I have chosen a SQL database rather than a NoSQL db,  
because relational databases are a mature technology that meet most of my requirements.

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

## Next Steps

* Add missing tests
* Refactor Deployment: use continuous deployment instead of continuous delivery
* Experimental: re-write the domain in `F#` since it is a better domain language

## Future Extensions

### Management Api

Build a management api for users and accounts.  
Do not use api key authentication for this api.  
It should be secured by using OAuth2.0 and Microsoft as service provider.  

### Frontend (needs to be learnt)

Create an angular app that is responsible for

* managing accounts and users
* working with releases and changes
* user registration with Microsoft accounts

### Existing Api

Extend authentication to deal with JWT access tokens.  
So it can be used with both access tokens and api keys.  
