<table>
<tr>
<td width="80%"><a href="../../../README.md"><img src="../../../docs/X2a.png" alt="Xigadee"></a></td>
<td width = "*" align="right"><img src="../../../docs/smallWIP.jpg" alt="Sorry, I'm still working here" height="100"></td>
</tr>
</table>

# The 15 minute Microservice

Xigadee is built to simplify the construction of a Microservice application. It does this my doing much of the heavy lifting regarding communication, task scheduling, logging etc, which allows you to concentrate on just the code and application logic that you need for your application.

This example gives a quick introduction to building a Microservice based application using the Xigadee framework.

For this example, we'll host the API service within a console application. 
We'll construct an API based Microservice that can persist an entity, 
using Create, Read, Update and Delete (CRUD) operations in memory through a simple set of RESTful API call.

First we are going to create a new console application and to this application add the Xigadee [NuGet](https://packages.nuget.org/packages/Xigadee) library to you project and add the following line in the _using_ section.
```C#
using Xigadee;
```
This server will be our back end server that receives requests from a front-end API service.


```C#
static void Main(string[] args)
{
    var server = new MicroservicePipeline("Server");

    server.Start();

    server.Stop();
}
```

<table><tr> 
<td><a href="http://www.hitachiconsulting.com"><img src="../../../docs/hitachi.png" alt="Hitachi Consulting" height="50"/></a></td> 
  <td>Created by: <a href="http://github.com/paulstancer">Paul Stancer</a></td>
  <td><a href="../../../README.md">Home</a></td>
</tr></table>
