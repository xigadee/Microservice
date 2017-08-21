![Xigadee](/docs/X2a.png)

Xigadee is an open-source Microservice framework, developed by [Paul Stancer](https://github.com/paulstancer) and [Guy Steel](https://github.com/guysteel) at Hitachi Consulting, and released under the Apache 2 license by Hitachi Consulting in 2016. You are free to use it within your own commercial applications without restriction. 

The framework is a result of our experience - and frustration - over the past five years, in building large-scale distributed cloud applications for our enterprise customers.

We found that when constructing Microservices, we could spend as much time on building and testing the repeatable "plumbing" code (messaging, monitoring, communication, security etc.) as we did on the actual application business logic. 

So our goal with Xigadee is to solve that challenge. To provide a consistent development approach - and more importantly a set of reusable tools – that we can apply to any type of Microservice application, while removing the drudgery and overhead of "re-inventing the Microservice-wheel", each time we construct a new distributed application.

Xigadee is still a work-in-progress, we are currently getting ready to release 1.1 of the Framework. We are still working on improving the code, improving the unit-test coverage, adding new features, and providing more detailed documentation.

## A quick demonstration

The Xigadee libraries are built using Microsoft .NET technologies, and have specific accelerators for targeting Platform-as-a-Service (PaaS) technologies in the Azure stack.

All the libraries utilise a simple declarative programming model to aid in the construction of the Microservice (see the [15-minute Microservice](Src/Xigadee.Platform/_Docs/fifteenminuteMicroservice.md) for more details). 

A quick sample of code from [this](Src/Test.Xigadee/Damples/PersistenceLocal.cs) unit-test shows how a Microservice can be quickly constructed within a few lines of code. 
```C#
    var p1 = new MicroservicePipeline("Local")
        .AddChannelIncoming("incoming")
            .AttachPersistenceManagerHandlerMemory(
                (Sample1 e) => e.Id, (s) => new Guid(s)
                , versionPolicy: ((e) => e.VersionId.ToString("N").ToUpperInvariant(), (e) => e.VersionId = Guid.NewGuid(), true)
                )
            .AttachPersistenceClient(out init);

    p1.Start();

    var sample = new Sample1(){Message="Hello mum"};

    //Create
    Assert.IsTrue(init.Create(sample).Result.IsSuccess);
    //Read
    Assert.IsTrue(init.Read(sample.Id).Result.IsSuccess);
```
This service creates a quick memory-based entity store for the POCO class, Sample1, that supports CRUD (Create/Read/Update/Delete) functions for the entity, with optimistic locking, and additional versioning and search methods, based on a key field (Id) and optional version field (VersionId) defined in the entity. 

If we were to use the [Xigadee Azure](Src/Xigadee.Azure/_docs/Introduction.md) library, we could replace the following method:
```C#
.AttachPersistenceManagerHandlerMemory(
```
with this method, which would switch it to use a DocumentDb (now CosmosDb) backed entity store:
```C#
.AttachPersistenceManagerDocumentDbSdk(
```                       
or this method to use a Azure Blob Storage collection instead:
 ```C#
.AttachPersistenceManagerAzureBlobStorage(
```
### Refectoring
As I mentioned earlier, Xigadee is designed to allow quick rapid application development, through easy refactoring of its pipeline based code.


### Communication

.

## Quick guides

If you are new to Microservice development, then the following links gives you an overview of Microservices and how to compose a Microservice based application.
* [What is a Microservice?](Src/Xigadee.Platform/_Docs/WhatIsAMicroservice.md)
* [An introduction to Xigadee.](Src/Xigadee.Platform/_Docs/Introduction.md)

Or if you want a quick introduction on how to build a new Microservice application using the Xigadee libraries, then read the following:
* [The 15-minute Microservice.](Src/Xigadee.Platform/_Docs/fifteenminuteMicroservice.md)


### NuGet Packages

Xigadee is made up of a set of libraries, which are listed below. They support different areas of Microservice functionality. These capabilities can be added to your project through the relevant [NuGet](https://www.nuget.org/packages?q=Tags%3A%22Xigadee%22) packages. 

* [Xigadee](Src/Xigadee.Platform/_Docs/Introduction.md) 
	- This is the core root library that is used to create Microservice and serverless based solutions. 
* [Xigadee Azure](Src/Xigadee.Azure/_docs/Introduction.md) 
	- This library allows for the seamless integration with many of the Azure platform PAAS services.
* [Xigadee Api Server](Src/Xigadee.Api.Server/_docs/Introduction.md)
	- This package allows for the rapid development of API based services, and includes integration with the Unity DI framework.
* [Xigadee Api Client](Src/Xigadee.Api.Client/_docs/Introduction.md)
	- This package speeds up the development of client libraries that consume API services.
* [Xigadee Console](Src/Xigadee.Console/_docs/Introduction.md)
	- This package is designed to help in building simple console based test harnesses, for your Microservice applications.
* [Xigadee Framework](Src/Xigadee.Framework/_docs/Introduction.md)
	- This package is used to provide deeper integration in to the Windows platform, and supports features which are not part of the .NET Standard library set.

To read what's new in the latest NuGet release packages, please click [here](/docs/whatsnew.md).

## Legal Stuff

_**Copyright © [Hitachi Consulting](http://www.hitachiconsulting.com) 2012-2017**_

Licensed under the Apache License, Version 2.0 (the "License").
You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
 
Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and limitations under the License.

## Feedback

Xigadee is in active development across a number of development projects. We welcome feedback and suggestions for future versions of the Framework. More importantly, if you are using the libraries and discover a [bug](https://github.com/xigadee/Microservice/issues/new), please let us know so we can fix it.

<table><tr> 
<td><a href="http://www.hitachiconsulting.com"><img src="docs/hitachi.png" alt="Hitachi Consulting" height="50"/></a></td>   
<td>Created by: <a href="http://github.com/paulstancer">Paul Stancer</a></td>
</tr></table>
