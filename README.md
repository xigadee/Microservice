# Xigadee Microservice Library

![Xigadee](/xigadee.png)

Xigadee is a Microservice framework, developed by Paul Stancer and Guy Steel. 

The libraries were developed to provide a consistent development approach to building complex scalable cloud based applications, particularly those targetting PAAS technologies.

It built on the .NET Framework, and is designed to enable the rapid development and deployment of scalable cloud based solutions.

## Introduction

Xigadee is made up of a number of distinct NuGet packages, which are listed below. They are arranged to support distinct part of a Microservice application.

* [Xigadee](Xigadee.Platform/_Docs/Introduction.md) - [Nuget](https://www.nuget.org/packages/Xigadee)
	- This is the core library that is used to create Microservice and serverless based solutions.
* [Xigadee Azure](Xigadee.Azure/_docs/Introduction.md) - [Nuget](https://www.nuget.org/packages/Xigadee.Azure)
	- This library allows for the detailed integration with many of the Azure platform PAAS services.
* [Xigadee Api Server](Xigadee.Api.Server/_docs/Introduction.md) - [Nuget](https://www.nuget.org/packages/Xigadee.Api.Server)
	- This package allows for the rapid development of API based services.
* [Xigadee Api Server Unity](Xigadee.Api.Server.Unity/_docs/Introduction.md) - [Nuget](https://www.nuget.org/packages/Xigadee.Api.Server.Unity)
	- This package provides helper functions to integrate Xigadee with a web based application that uses Unity as its Dependency Injection mechanism
* [Xigadee Api Client](Xigadee.Api.Client/_docs/Introduction.md) - [Nuget](https://www.nuget.org/packages/Xigadee.Api.Client)
	- This package speeds up the development of client libraries that consume API services.
* [Xigadee Api Console](Xigadee.Console/_docs/Introduction.md) - [Nuget](https://www.nuget.org/packages/Xigadee.Console)
	- This package is designed to help in creating simple console based test harnesses for Microservice applications.

## Legal Stuff

**Copyright Hitachi Consulting 2012-2017**

Licensed under the Apache License, Version 2.0 (the "License").
You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
 
Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and limitations under the License.