![Xigadee](/docs/X2a.png)

Xigadee is an open-source Microservice framework, developed by [Paul Stancer](https://github.com/paulstancer) and [Guy Steel](https://github.com/guysteel), and released under the Apache 2 license by Hitachi Consulting in 2016. You are free to use it within your own commercial applications without restriction. 

The framework is a result of our experience - and frustration - over the past five years, in building large-scale distributed cloud applications for our enterprise customers.

We found that when constructing Microservices, we could spend as much time on building and testing the repeatable "plumbing" code (messaging, monitoring, communication, security etc.) as we did on the actual application business logic. 

So our goal with Xigadee is to solve that challenge. To provide a consistent development approach; and more importantly - a set of reusable tools that we can apply to any type of Microservice application - while removing the drudgery and overhead of "re-inventing the Microservice-wheel" each time we construct a new distributed solution.

**_Please note: the current main branch of the code is being converted to support both .NET Standard 2.0 and .NET Framework 4.7 ready for release 2 of Xigadee. Ideally you should have Visual Studio 2017 v15.4 or later to build._**

## A quick demonstration

The Xigadee libraries are built using Microsoft's .NET technology, and have specific accelerators to target Platform-as-a-Service (PaaS) technologies in the Azure stack.

All the libraries utilise a simple declarative programming model to aid in the construction of the Microservice. 

A quick sample of code from [this](Src/Test/Test.Xigadee/Samples/PersistenceLocal.cs) unit test shows how a Microservice can be quickly constructed within a few lines of code. This code can be found in the '_PersistenceSingle_' method:
```C#
PersistenceClient<Guid, Sample1> repo;

var p1 = new MicroservicePipeline("Local")
    .AddChannelIncoming("request")
        .AttachPersistenceManagerHandlerMemory(
              keyMaker: (Sample1 e) => e.Id
            , keyDeserializer: (s) => new Guid(s)
            , versionPolicy: ((e) => e.VersionId.ToString("N").ToUpperInvariant(), (e) => e.VersionId = Guid.NewGuid(), true)
        )
        .AttachPersistenceClient(out repo)
        .Revert()
    ;

p1.Start();

var sample = new Sample1() { Message = "Hello mom" };
var id = sample.Id;
//Run a set of simple version entity tests.
//Create
Assert.IsTrue(repo.Create(sample).Result.IsSuccess);
//Read
var result = repo.Read(id).Result;
Assert.IsTrue(result.IsSuccess);
Assert.IsTrue(result.Entity.Message == "Hello mom");
```
This service creates a quick memory-based entity store for the POCO class, Sample1, that supports CRUD (Create/Read/Update/Delete) functions for the entity, with optimistic locking, and additional versioning and search methods, based on a key field (Id) and optional version field (VersionId) that are defined in the entity. 

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

If you want a more in-depth explanation of how to build a new Microservice application using the Xigadee libraries, then jump to the following article: [The 15-minute Microservice.](Src/Xigadee.Platform/_Docs/fifteenminuteMicroservice.md)

## Feedback
Xigadee is in active development across a number of development projects, and is still very much a work-in-progress; we are still improving the code, extending the unit-test coverage, adding new features, and providing more detailed documentation and code examples.

We have recently shipped release 1.1 of the Framework, which has some key improvements in creating custom application logic. Our next version will be 2.0 which will be built under the .NET Standard 2.0, which will allow Xigadee applications to work with both traditional .NET Framework libraries, but also to use the new .NET Core multi-platform capabilities, such as Linux and ARM based systems.

 We welcome feedback and suggestions for future features of the Framework. More importantly, if you are using the libraries and discover a bug, please report it [here](https://github.com/xigadee/Microservice/issues/new) so we can track and fix it.

## Quick guides

If you are new to Microservice development, then the following links gives you an overview of the technology and how a Microservice based application is composed.
* [What is a Microservice?](Src/Xigadee.Platform/_Docs/WhatIsAMicroservice.md)
* [An introduction to Xigadee.](Src/Xigadee.Platform/_Docs/Introduction.md)

## NuGet Packages

Xigadee is made up of a set of libraries, which are listed below. They support different areas of Microservice functionality. These capabilities can be added to your project through the relevant [NuGet](https://www.nuget.org/packages?q=Tags%3A%22Xigadee%22) packages. 

* [Xigadee](Src/Xigadee.Platform/_Docs/Introduction.md) 
	- This is the core root library that is used to create Microservice based solutions. 
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

Parts of Xigadee are the copyright of [Hitachi Consulting](http://www.hitachiconsulting.com) 2012-2017. 

Xigadee is released in its entirety under the Apache License, Version 2.0 (the "License").
You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
 
Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and limitations under the License.


<table><tr> 
<td>Created by: <a href="http://github.com/paulstancer">Paul Stancer</a></td>
</tr></table>
