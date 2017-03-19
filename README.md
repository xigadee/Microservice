![Xigadee](/docs/xigadee.png)

Xigadee is an open-source Microservice framework, developed by [Paul Stancer](https://github.com/paulstancer) and [Guy Steel](https://github.com/guysteel) at Hitachi Consulting. The framework was developed over a number of years, based on the excperience of building large-scale distributed cloud applications. 

The libraries are designed to provide a simplified and consistent approach to building complex scalable Microservice-based cloud applications, particularly those targetting PAAS technologies.

It is built using Microsoft .NET technologies, and has deep integration in to the Azure technology stack. Xigadee is particularly suited to the serverless 

## Introduction

The aim of Xigadee is to provide a simple extensible framework that allows a development team to create a set of services that naturally support the scale out necessary in a cloud based, without the complexity of implementing the complex concurrent coding needed.

* [The 15 minute Microservice](Xigadee.Platform/_Docs/fifteenminuteMicroservice.md)

## Packages

Xigadee is made up of a number of distinct NuGet packages, which are listed below. They support different areas of a Microservice application.

* [Xigadee](Xigadee.Platform/_Docs/Introduction.md) 
	- This is the core library that is used to create Microservice and serverless based solutions.
* [Xigadee Azure](Xigadee.Azure/_docs/Introduction.md) 
	- This library allows for the seamless integration with many of the Azure platform PAAS services.
* [Xigadee Api Server](Xigadee.Api.Server/_docs/Introduction.md)
	- This package allows for the rapid development of API based services, and includes integration with the Unity DI framework.
* [Xigadee Api Client](Xigadee.Api.Client/_docs/Introduction.md)
	- This package speeds up the development of client libraries that consume API services.
* [Xigadee Console](Xigadee.Console/_docs/Introduction.md)
	- This package is designed to help in building simple console based test harnesses, for your Microservice applications.

## Legal Stuff

**Copyright Hitachi Consulting 2012-2017**

Licensed under the Apache License, Version 2.0 (the "License").
You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
 
Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and limitations under the License.

## Feedback

Xigadee is in active development across a number of development projects, and we welcome feedback and suggestions for future versions of the Framework, and more importantly bug reports.

![Hitachi](/docs/hitachi.png)
