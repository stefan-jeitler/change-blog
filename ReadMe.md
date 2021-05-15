# ChangeTracker - empower your releases

ChangeTracker is web service that enables you to keep track of your releases and changes.  

Due to the continuous movement towards microservices releases are harder to track.  
Often, monoliths gets subdivided into many micro/nano services, each of them are deployed independently with its own versioning.  

Imagine you're a product owner who is responsible for many products.  
In order to communicate the changes to the customer you need to be up-to-date about the releases.  
Maybe you talk to the developer directly or call an api info endpoint to get the latest deployed version.  
After that, you check what's inside this release in your ticket system to be informed about the latest changes.  

This can be quite cumbersome with an increasing number of products.  

A possible solution is to invert the dependencies.  
The management team where the product owner belongs to should not depend on developers,  
both should dependend on a web service.  

![Dependencies](./docs/assets/ChangeTracker.png)  

The development team pushes their changes automatically during the deployment with all information needed by the management team.  

## Disclaimer

This is a side-project and should not be used in a prodcutive environment.  
