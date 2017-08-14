<table>
<tr>
<td width="80%"><a href="../../../README.md"><img src="../../../../docs/X2a.png" alt="Xigadee"></a></td>
<td width = "*" align="right"><img src="../../../docs/smallWIP.jpg" alt="Sorry, I'm still working here" height="100"></td>
</tr>
</table>

# The Command pattern
The command is the key design pattern for the Xigadee system. All system functionality should be implemented though a set of command objects.

Xigadee is based on a request/response messaging architecture, with commands being the termination points for the interplay of messages.

Commands can send messages between other commands in the same Microservice, or to commands in remote Microservices using channel and communication protocols. The complexity of doing this is abstracted away from the command logic.

Commands also allow functionality to be shared across services and other system when required.

#### Serialization
Implementers can choose to abstract the serialzation of incoming objects away from the implementation of the command by using method attributes.

### Command Methods
At its simplest, the command methods receive an incoming Payload object and return zero or many payload objects as a response.

### Jobs

### MasterJobs

## Policy

## Attributes
Within the command, you can use attributes to register methods that can receive requests, or initiate poll actions.
#### [CommandContract] Attribute

#### [JobSchedule] Attribute

#### [MasterJobCommandContract] Attribute

#### [MasterJobSchedule] Attribute

## Outgoing Requests

## Events

## Statistics


<table><tr> 
<td><a href="http://www.hitachiconsulting.com"><img src="../../../docs/hitachi.png" alt="Hitachi Consulting" height="50"/></a></td> 
<td>Created by: <a href="http://github.com/paulstancer">Paul Stancer</a></td>
  <td><a href="https://www.nuget.org/packages/Xigadee">NuGet Package</a></td>
  <td><a href="../../../README.md">Home</a></td>
</tr></table>
