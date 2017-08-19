![Xigadee](/docs/X2a.png)

Xigadee is a Microservice framework, developed by [Paul Stancer](https://github.com/paulstancer) and [Guy Steel](https://github.com/guysteel) at Hitachi Consulting. 
The framework is based on our experience, and the knowledge gained, 
in building large-scale distributed cloud applications for our clients over the past four years. 

We found when constructing a Microservice based system, that typically much of the work is spent on building and testing the application "plumbing", i.e. messaging, monitoring, communication etc., instead of on the actual application logic. Xigadee's goal is to solve that problem. It provides a consistent approach, and a set of reusable tools, that can be applied to many types of Microservice application.

Xigadee provides a simple declarative programming model to speed up the construction of a Microservice (see the [15-minute Microservice](Src/Xigadee.Platform/_Docs/fifteenminuteMicroservice.md) for details). 
 
The libraries are built using Microsoft .NET technologies, and we have specific accelerators for targeting Platform-as-a-Service (PaaS) technologies in the Azure stack.

Xigadee is now open-source; released under the Apache 2 license. You are free to use it within your own commercial applications without restriction. 

To read what's new in the latest NuGet release packages, please click [here](/docs/whatsnew.md).

## Quick guides

If you are new to Microservice development, then the following links gives you an overview of Microservices and how to compose a Microservice based application.
* [What is a Microservice?](Src/Xigadee.Platform/_Docs/WhatIsAMicroservice.md)
* [An introduction to Xigadee.](Src/Xigadee.Platform/_Docs/Introduction.md)

Or if you want a quick introduction on how to build a new Microservice application using the Xigadee libraries, then read the following:
* [The 15-minute Microservice.](Src/Xigadee.Platform/_Docs/fifteenminuteMicroservice.md)

### Packages

Xigadee is made up of a number of distinct NuGet packages, which are listed below. They support different areas of Microservice functionality. These capabilities can be added to your project through the relevant [NuGet](https://www.nuget.org/packages?q=Tags%3A%22Xigadee%22) packages. Currently Xigadee is in a pre-release state, but we are working to complete the documentation, and improve the code test coverage, so that we can provide a base line set of releases.

* [Xigadee](Src/Xigadee.Platform/_Docs/Introduction.md) 
	- This is the core library that is used to create Microservice and serverless based solutions.
* [Xigadee Azure](Src/Xigadee.Azure/_docs/Introduction.md) 
	- This library allows for the seamless integration with many of the Azure platform PAAS services.
* [Xigadee Api Server](Src/Xigadee.Api.Server/_docs/Introduction.md)
	- This package allows for the rapid development of API based services, and includes integration with the Unity DI framework.
* [Xigadee Api Client](Src/Xigadee.Api.Client/_docs/Introduction.md)
	- This package speeds up the development of client libraries that consume API services.
* [Xigadee Console](Src/Xigadee.Console/_docs/Introduction.md)
	- This package is designed to help in building simple console based test harnesses, for your Microservice applications.
* [Xigadee Framework](Src/Xigadee.Framework/_docs/Introduction.md)
	- This package is used to provide deeper integration in to the Windows platform.

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
