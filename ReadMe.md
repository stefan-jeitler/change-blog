# ChangeBlog - enhance your change logs

ChangeBlog is a web service that enables you to keep track of your releases and changes.

Due to the continuous movement towards microservices, releases are harder to track.  
Often, monoliths get subdivided into many micro/nano services,  
each of them is deployed independently with its own versioning.

Imagine you're a product owner who is responsible for many products.  
In order to communicate changes to the customer, you need to be up-to-date about releases.  
Maybe you talk to the developer directly or call an api info endpoint to get the latest deployed version.  
Subsequently, you check what's inside this release in your ticket system to be informed about the latest changes.

This can be quite cumbersome with an increasing number of products.

A possible solution is inverting the dependencies.  
The management team should not depend on developers,
both should depend on a web service.

![Dependencies](https://changeblog.blob.core.windows.net/images/ChangeBlog.png)

The development team automatically pushes their changes during deployment with all the information the management team needs.

[![Build Status](https://dev.azure.com/stefanjeitler/ChangeBlog/_apis/build/status/stefan-jeitler.changeblog?branchName=main)

## Table of Contents

* [Basic Concept](#basic-concept)
* [Key Features](#key-features)
* [Architecture](#architecture)
* [Testing](#testing)
* [CI/CD](#ci-cd)
* [Environments](#environments)
* [Database](#database)
* [Next Steps](#next-steps)
* [Future Extensions](#future-extensions)
* [Credits](#credits)

## Basic Concept

### Account

An Account is a grouping of users and products.  
User roles can be assigned on account and product level.  
Product roles have precedence over account roles.

For instance, a user with a developer role on account level and a support role for one specific product within the account has for all account products developer permissions except the one specific product.  
This is restricted to support permissions.

It works the other way around as well.  
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

Currently, the following user information is stored

* Email
* Firstname
* Lastname
* Timezone (olson id)

### Product

A product is the software on which developers work.  
It has no end date, but it can be closed.  
By closing a product, you stop working on that software.  
The product will still exist, but it becomes read-only with all its versions and change logs.

### Version

A version is a unique string assigned to a product and marks an important stage.  
There are three states that a version can have.

* Not Released and Not Deleted
* Released
* Deleted

The last two make versions read-only.  
Version properties including the change logs can be modified as long it hasn't been deleted or released.  
Deleted versions can still be fetched from the api.  
The appropriate endpoints provide a switch **IncludeDeleted** to include those versions in the response.

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

![Concept](https://changeblog.blob.core.windows.net/images/ChangeBlogConcept.png) 

## Key Features

* Role-based access control
* Multitenancy (single database, single schema)
* Single sign-on (Microsoft Identity for the time being)
* Full-text Search for versions and change logs
* Zero Downtime Deployment

## Architecture

The architecture of the app is strongly influenced by

* Robert C. Martin's book Clean Architecture
* Mark Seemann's book Dependency Injection Principles, Practices and Patterns
* Plainionist's article series [Implementing-Clean-Architecture](http://www.plainionist.net/Implementing-Clean-Architecture/)
* candied_orange's [answers](https://softwareengineering.stackexchange.com/search?q=user:131624+[clean-architecture]) on [softwareengineering.stackexchange](https://softwareengineering.stackexchange.com/)

### My take on the Clean Architecture

I would like to point out that I don't implement the Clean Architecture strictly.  
There are two major differences.

**First**  
My solution contains only three circles and not four as shown [here](https://blog.cleancoder.com/uncle-bob/images/2012-08-13-the-clean-architecture/CleanArchitecture.jpg).
I brought together the two outermost circles.

* Domain: **Enterprise Business Rules**
* Application: **Application Business Rules**
* WebAPI and DataAccess: **Interface Adapters** and **Framework & Drivers**

The main reason for this decision was convenience.  
Controllers in the Clean Architecture are located in the **Interface Adapters** circle.  
Since dependencies can only point inwards, this layer is not allowed to know about **Frameworks & Drivers**.  
`ASP.NET` is such a **Framework** and implementing it strictly means controllers must not know about `ASP.NET`.  
Some things I would highly miss in my controllers are

* HttpContext
* ActionResult
* Response methods like `Ok`, `NotFound`, `BadRequest`, ...
* Model validation
* Routing

So it was clear to not follow the clean architecture here.  
Maybe I don't keep the Framework at arm's length in my solution,  
but `ASP.NET` is a mature web framework and I can rely upon it.

**Second**  
Controllers and Presenters.

In theory, controllers and presenters don't know anything about each other and controllers return nothing.  
AFAIK this is not possible with `ASP.NET` actions, they have to return something, e.g. `ActionResult`.  
Therefore, I'm not able to follow the Clean Architecture here as well.

In the simplest case interactors return the data to controllers directly and presenters are not involved.  
This fits best if the use-case itself is simple, e.g. the cyclomatic complexity is low.  
But for the more advanced use-cases presenters are quite handy.  
They enable me to create a verbose output without using exceptions to deal with the Unhappy Path.

The concrete presenter will be created in the controller and passed to the interactor through the input boundary.  
This introduced a dependency from controllers to presenters and the controller decide which presenter to use.

Maybe I can't see it, but let controllers know about presenters is not that bad.  
However, you might wonder why not inject presenters?

Mark Seemann states in his book:
>"Volatile Dependencies are the focal point of DI. It’s for Volatile
Dependencies rather than Stable Dependencies that you introduce Seams
into your application. Again, this obligates you to compose them using DI."

I don't see presenters as being that volatile.

### Dependencies

Modern applications rely heavily on third party packages.  
Adding a new package is easy and often seems to have no drawbacks.  
But the surprise may come later if you use it without care throughout your codebase.

Let me do a little case study about dependencies.  
I will start at the domain layer that tries to solve the domain problem.  

What dependencies are there?

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>

</Project>
```

As you can see, there are no outgoing dependencies.  
It lacks of `PackageReference`s and `ProjectReference`s.

This makes the domain stable and it can be referenced by components which are less stable.  
What I call domain is the innermost circle that is supposed to be referenced by other components, otherwise it would be useless.   
But be aware there is a dependency you won't see at first glance.  
It's the .NET Base Class Library(BCL).

Uncle Bob Martin states in Clean Architecture

>"There are some frameworks that you simply must marry. If you are using C++, for
example, you will likely have to marry STL—it’s hard to avoid. If you are using Java,
you will almost certainly have to marry the standard library."

So I have to marry the BCL.  
All in all, the domain layer is not that bad.  
However, there are still things I don't like, but that's another topic.

The application layer is the next one towards the app's details.  
According Uncle Bob, you will find here the _Dance of the entities_.

Let's take a look at the project file.

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net6.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\ChangeBlog.Domain\ChangeBlog.Domain.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="CSharpFunctionalExtensions" Version="*" />
    <PackageReference Include="Microsoft.Extensions.Caching.Abstractions" Version="*" />
    <PackageReference Include="NodaTime" Version="*" />
  </ItemGroup>

</Project>
```

The first outgoing dependency you'll see is the domain layer which can be referenced safely from here.  
But what about nuget packages?  
I will only discuss two of them to illustrate the basic problem that comes with such dependencies.  

**NodaTime** is well encapsulated [here](https://dev.azure.com/stefanjeitler/_git/ChangeBlog?path=%2Fsrc%2FChangeBlog.Application%2FExtensions%2FDateTimeExtensions.cs&version=GBmain&line=8&lineEnd=9&lineStartColumn=1&lineEndColumn=1&lineStyle=plain&_a=contents). This is the only place where it is used.  
Replacing NodaTime with a different time zone library is quite easy,  
only the implementation in the extension method has to be swapped out.  
I think such libraries do not harm applications.  
They can be used without concerns if they are abstracted.

In contrast, **CSharpFunctionalExtensions** is widely used in the application layer  
and it isn't hidden, the api is exposed to the app directly.  
Hiding this library is too much work, I would have to create too many methods.  
But what if the author of CSharpFunctionalExtensions introduces breaking changes  
to one of his methods that is frequently used in my app.  
After updating the package, Visual Studio will give me plenty of compile time errors.  
Fixing all usages would be a pain in the ass.  

Using libraries this way in the inner circles gives me a bad feeling.  
I use it anyway because the library doesn't do too much — is more or less simple —  
and so I do not expect breaking changes.  
I hope I'm not wrong.  

**Update 2021-10-02**:  
It happened or something like that.  
In the version 2.21.0 of **CSharpFunctionalExtensions**  
the `Value` Property of the `Maybe` type got marked as deprecated and  
I received over 150 Warnings after building the app.  
`GetValueOrThrow()` should be used instead of `Value` from now on.  
In order to get rid of all Warnings I had to update every single usage.  
It was done in a short time, in about 15 minutes.  

Now, we come to my outermost circle that contains all the details.  

I do not print any project file here because that would be too much.  
This layer is not supposed to contain any business logic.  

... tbd

## Testing

Some parts of the app were developed using TDD.  
This includes the Domain layer and the commands in the Application layer.

Sometimes it's quite hard to do TDD,
especially when it comes to components where the first design is hardly the final one.  
An example is the authorization mechanism in the Web Api.  
While developing it, I was quite unsure whether I'm on the right track.  
In these situations I like to experiment and this often leads to frequent changes in my design.

On the other hand, pure functions are qualified for TDD.  
Pure functions have often a stable api that is not likely to change frequently.  
This prevents me from the **Oh No** moment where you change something in the api of your implementation and many tests will no longer compile.

However, there is one test approach I like the most.

**Test First**  
By Test First I mean Jon Skeet's ~~definition~~ description https://stackoverflow.com/a/334815/13842370

## CI CD

Every commit pushed to the remote repo triggers the **Continuous Integration** stage
where the app is built and all tests are executed.

The **Continuous Delivery** stage will run afterwards if the commit was tagged.  
By doing so, the app gets deployed to the staging environment
and after a manual approval to the production environment.

Tag names must be a valid SemVer 2.0.0.  

The api can be deployed without a downtime.  
But it works only if there are no breaking changes in the db updates.  
The app will be stopped during deployment if there are breaking changes.

I call these two deploy strategies **Rolling Deployment** and **Breaking Deployment**.  
Noticing breaking changes is easy since db update versions are made of semantic versions.  
This is done automatically in the release pipeline.  

### CI/CD Overview

![CICD](https://changeblog.blob.core.windows.net/images/CI_CD.png)

## Environments

**Testing**  
This environment consists of a database only that is used by integration tests.

**Staging**  
Is a replica of the production environment.  
Uses the same Docker registry and App Service Plan as the production system.

[Staging](https://app-change-blog-staging.azurewebsites.net/)
[WIP: Management App](https://app-change-blog-management-staging.azurewebsites.net/)

**Production**  
Actual app

[Production](https://app-change-blog.azurewebsites.net/)

Demo User

&nbsp;&nbsp;&nbsp;&nbsp;ApiKey: AwqQoytQJ8MUS1OOx4ta  

&nbsp;&nbsp;&nbsp;&nbsp;Assigned Roles:

* DefaultUser
* Developer

## Database

I have chosen a SQL database rather than a NoSQL db,  
because relational databases are a mature technology that meet most of my requirements.

[Database Schema](https://changeblog.blob.core.windows.net/images/ChangeBlogDbSchema.png)

### DbConnection

There are two rules of thumb when working with db connections

* Open it as late as possible (during an http request)
* Close it as soon as possible

**How to get a db connection?**

Two types were designed to work with db connections.

* `Func<IDbConnection>`
* `IDbAccessor`

These two types can be injected into the constructor,
but only in the data access layer.

`Func<IDbConnection>` should be used in situations where no transaction is required.  
An example is the authentication handler that verifies the user's identity.  
This will never be part of a transaction.

Things to keep in mind when working with `Func<IDbConnection>`.

* Wrap the connection in a `using` block since it's an ephemeral disposable
* Does not work with `IUnitOfWork`
* Can be used in concurrent situations

`IDbAccessor` contains only the property `DbConnection` that can be used safely  
without worrying about opening or closing/disposing connections or transactions.  
This is done by Dapper, the DbSession and the DI Container.

`IDbAccessor` was designed to deal with transactions  
that are controlled by **Unit of Works** in the application layer.

Things to keep in mind when working with `IDbAccessor`

* Do not dispose connections
* Do not open or close connections
* Do not begin transactions
* Do not use it in concurrent scenarios
* Use it with Dapper only

### Performance/Utilization

Imagine you're developing a feature that requires a new table in your db.  
You create the table by the book and your new relation is in the third normal form.  
You think to yourself, well done.  

After deploying to production a DBA tells you that querying your table leads  
to high cpu utilization on the database server.  
The query is among the top 10 most resource consuming queries and  
if you can't fix it more cpu cores must be purchased.  

The feature you have developed was considered as a cross-cutting concern and  
was placed in an `ASP.NET` ActionFilter that is invoked on every http request.  

The solution was easy.  
The columns in the where clause were not indexed and  
every execution caused a full table scan.  
Adding a proper index solved the perfomance problem.  

Causing such issues in a high load environment is really annoying.  
To inherently avoid those issues I had the following in mind during development

* Limit result sets using seek paging
* Create indices that reflect access paths

However, most of the queries result in a full table scan right now.  
The optimizer does not pick an execution plan that uses an index.  
I expect this will change when there is more data in the database.  
It's hard to force the optimizer to use an index execution plan  
when there are only 20 rows in the table even if the statistics are up to date.

## Next Steps

* ~~Add mis~~sing tests
* Refactor Deployment: use continuous deployment instead of continuous delivery

## Future Extensions

### Management Api

Build a management api for users and accounts.  
Do not use api key authentication for this api.  
It should be secured by using OAuth 2.0 and Microsoft as identity provider.

### Frontend

Create an angular app that is responsible for

* user registration with Microsoft accounts
* managing accounts and users
* working with releases and changes

### Existing Api

Extend authentication to deal with JWT access tokens.  
So it can be used with both access tokens and api keys.

## Credits

<img src="https://changeblog.blob.core.windows.net/images/JetBrains-Logo.png" alt="drawing" width="125"/>  

Thanks go to JetBrains for providing an Open Source development license.
