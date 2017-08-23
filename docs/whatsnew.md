![Xigadee](X2a.png)

# What's new?

This section details the Xigadee release versions with the features added since the initial release.

### Pre-release - 2.0.x.x (ongoing)

This version will be porting key components over to .NET Standard 2.0 to enable Xigadee to be used in .NET Core applications. We will also be supporting .NET Framework 4.6 and higher as well.

### Current Release - 1.1.x.x (August 13, 2017)

This version is currently in development and contains additional support for rate limiting within the Xigadee core engine, for systems that experience heavy load. 

There are also a number of bug fixes around high-volume performance issues that are currently pending a full release. 

There are also changes around the Command object, specifically the introduction of schedule attributes to simplify the setting up of timed poll jobs. Command method and schedule attributes have also been extended to master job to simplify configuration. There are also some small breaking changes for Command event arguments, but these should not affect normal operation.

### Previous Release - 1.0.17160.7 (June 9, 2017)

This is the current supported release. It contains all the core functionality for the Xigadee framework, and has been used in a number of internal and commercial projects. There are currently 7 NuGet packages in this release, which are detailed below:

- [Xigadee](https://www.nuget.org/packages/Xigadee/1.0.17160.7)
- [Xigadee.Api.Client](https://www.nuget.org/packages/Xigadee.Api.Client/1.0.17160.7)
- [Xigadee.Api.Server](https://www.nuget.org/packages/Xigadee.Api.Server/1.0.17160.7)
- [Xigadee.Api.Server.Unity](https://www.nuget.org/packages/Xigadee.Api.Server.Unity/1.0.17160.7)
- [Xigadee.Azure](https://www.nuget.org/packages/Xigadee.Azure/1.0.17160.7)
- [Xigadee.Framework](https://www.nuget.org/packages/Xigadee.Framework/1.0.17160.7)
- [Xigadee.Console](https://www.nuget.org/packages/Xigadee.Console/1.0.17160.7)


<table><tr> 
<td><a href="http://www.hitachiconsulting.com"><img src="hitachi.png" alt="Hitachi Consulting" height="50"/></a></td> 
<td>Created by: <a href="http://github.com/paulstancer">Paul Stancer</a></td>
  <td><a href="../README.md">Home</a></td>
</tr></table>
