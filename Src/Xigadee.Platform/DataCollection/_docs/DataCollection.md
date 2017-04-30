<table>
<tr>
<td width="80%"><a href="../../../README.md"><img src="../../../../docs/X2a.png" alt="Xigadee"></a></td>
<td width = "*" align="right"><img src="../../../docs/smallWIP.jpg" alt="Sorry, I'm still working here" height="100"></td>
</tr>
</table>

# Data Collection and Monitoring

Data collection is one of the key requirements for a Microservice based application. This is due to the distributed nature of the solutions. 
It is critical that each key component can be monitored independently.

Xigadee has an extensible and extensive logging and monitoring capability. This is built around the Data Collection functionality.

## Events

- Boundary
   - Boundary events are generated when a message enters or leaves a Microservice. There are different type of message based on the technology or protocol being used.

- Dispatcher
  - Dispatcher Events are used to trace the processing of an incoming message within the Microservice.

- Event Source
  - EventSource Events record a change of state within the Microservice, this is of particular relevance to the Persistence Commands where they use this to record the Create, Update and Delete requests for an Entity.

- Log

- Resource

- Security

- Telemetry

<table><tr> 
<td><a href="http://www.hitachiconsulting.com"><img src="../../../docs/hitachi.png" alt="Hitachi Consulting" height="50"/></a></td> 
<td>Created by: <a href="http://github.com/paulstancer">Paul Stancer</a></td>
  <td><a href="https://www.nuget.org/packages/Xigadee">NuGet Package</a></td>
  <td><a href="../../../README.md">Home</a></td>
</tr></table>
