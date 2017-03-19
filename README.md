![Xigadee](/xigadee.png)

Xigadee is a Microservice framework, developed by Paul Stancer and Guy Steel at Hitachi Consulting. 

The libraries are designed to provide a consistent approach when building complex scalable cloud based applications, particularly those targetting PAAS technologies.

The framework is built on Microsoft .NET technologies, and has deep integration in to many of the Azure technology stack.

## Introduction

Xigadee is made up of a number of distinct NuGet packages, which are listed below. They are arranged to support distinct part of a Microservice application.

* [Xigadee](Xigadee.Platform/_Docs/Introduction.md) 
	- This is the core library that is used to create Microservice and serverless based solutions.
	- [Nuget](https://www.nuget.org/packages/Xigadee)
* [Xigadee Azure](Xigadee.Azure/_docs/Introduction.md) 
	- This library allows for the detailed integration with many of the Azure platform PAAS services.
	- [Nuget](https://www.nuget.org/packages/Xigadee.Azure)
* [Xigadee Api Server](Xigadee.Api.Server/_docs/Introduction.md)
	- This package allows for the rapid development of API based services.
	- [Nuget](https://www.nuget.org/packages/Xigadee.Api.Server)
* [Xigadee Api Server Unity](Xigadee.Api.Server.Unity/_docs/Introduction.md) 
	- This package provides helper functions to integrate Xigadee with a web based application that uses Unity as its Dependency Injection mechanism
	- [Nuget](https://www.nuget.org/packages/Xigadee.Api.Server.Unity)
* [Xigadee Api Client](Xigadee.Api.Client/_docs/Introduction.md)
	- This package speeds up the development of client libraries that consume API services.
	- [Nuget](https://www.nuget.org/packages/Xigadee.Api.Client)
* [Xigadee Api Console](Xigadee.Console/_docs/Introduction.md)
	- This package is designed to help in creating simple console based test harnesses for Microservice applications.
	- [Nuget](https://www.nuget.org/packages/Xigadee.Console)

## Legal Stuff

**Copyright Hitachi Consulting 2012-2017**

Licensed under the Apache License, Version 2.0 (the "License").
You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
 
Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and limitations under the License.

## Feedback

Xigadee is in active development across a number of development projects, and we welcome feedback and suggestions for future versions of the Framework, and more importantly bug reports. 

@paulstancer @guysteel