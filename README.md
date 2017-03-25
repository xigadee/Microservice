![Xigadee](/docs/Xigadee2.png)
Xigadee is a Microservice framework, developed by [Paul Stancer](https://github.com/paulstancer) and [Guy Steel](https://github.com/guysteel) at Hitachi Consulting. The framework has been in development for a number of years, and is based on our experience - and more importantly our pain - in building large-scale distributed cloud applications for our clients. 

The libraries are designed to simplify, and provide a consistent approach to our consulting teams, in building modern applications that target Platform-As-A-Service (PAAS) technologies. 

Xigadee is now open-source, released under the Apache 2 license, and you are free to use it within your own commercial applications without restriction. It is built using Microsoft .NET technologies, and has deep integration in to the Azure technology stack. 

## Introduction

To get started, please read the links below. These will give you an understanding of how we have put Xigadee toghther, and how you can use it in your software projects.

* [What is a Microservice?](Xigadee.Platform/_Docs/WhatIsAMicroservice.md)
* [The 15-minute Microservice - an introduction to the configuration pipeline.](Xigadee.Platform/_Docs/fifteenminuteMicroservice.md)

## Packages

Xigadee is made up of a number of distinct NuGet packages, which are listed below. They support different areas of Microservice functionality. These packages can be added to your project through the relevant [NuGet](https://www.nuget.org/packages?q=Tags%3A%22Xigadee%22) packages. These packages are currently in a pre-release state, but we are working to complete the documentation, and improve the code test coverage, so that we can provide a full set of releases.

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
* [Xigadee Framework](Xigadee.Framework/_docs/Introduction.md)
	- This package is used to provide deeper integration in to the Windows platform.

## Legal Stuff
![Hitachi](/docs/hitachi.png)

_**Copyright © Hitachi Consulting 2012-2017**_

Licensed under the Apache License, Version 2.0 (the "License").
You may obtain a copy of the License at: http://www.apache.org/licenses/LICENSE-2.0
 
Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and limitations under the License.

## Feedback

Xigadee is in active development across a number of development projects, and we welcome feedback and suggestions for future versions of the Framework, and more importantly bug reports.

_[Paul Stancer](https://github.com/paulstancer)_
