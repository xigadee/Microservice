<table>
<tr>
<td width="80%"><a href="../../README.md"><img src="../../docs/X2a.png" alt="Xigadee"></a></td>
<td width = "*" align="right"><img src="../../docs/smallWIP.jpg" alt="Sorry, I'm still working here" height="100"></td>
</tr>
</table>

# Xigadee library - an introduction

Xigadee is an extensible Microservice framework that can be used to build modern scalable applications using Platform-As-A-Service technologies.

It is made up of a number of key components. In this section, I will outline the basic building blocks of a Xigadee Microservice, and explain how you can use them to build your application.

<img src="Images/Xigadee.png" alt="Xigadee" width="800"/>

## The message flow

Xigadee works as a message processing system. Messages are passed between the Microservices throough the channels. Once a message is received by a Microservice, it will be routed to the relevant command object through the path documented below.

<img src="Images/MessageFlow.png" alt="Message Flow" width="800"/>

1. Message is received through the communication channel and is passed to the Task Manager for processing.
2. The Task Manager will queue the message until a processing slot is available. Once a slot is free the message is passed to the Dispatcher.
3. The Dispatcher will match the message to the commands available in the Microservice through the destination information in the message header. The message will then be passed to the commands that can accept the message.
4. A response message is received from the command and is then passed to the relevant channel for transmission. Response messages can also be passed to additional commands within the same Microservice for additional processing. Commands can return zero or many response messages to each request message.
5. Finally the response message is passed to the channel specified in its destination and is transmitted.

### The message

<img src="Images/MessageProcessing.png" alt="Message Flow" width="600"/>

1. The binary payload is received by the Listener. 
2. The listener creates a ServiceMessage object with the binary payload and adds the relevant service message metadata. The metadata will include information such as the destination command, and the response destination if required.
3. Finally the service message is passed to the TaskManager.

### The channels

Channels are an important concept in Xigadee. They are used to route information between and within Microservice

### The Listeners and the Senders


### Asynchronous messaging

### Synchronous messaging

### Priority lanes

## The command object

### Policy

### Statistics

### The types of command

#### Persistence

#### Master Jobs

#### Command Initiators

## Serialization

## Security

## Data Collection

## The configuration pipeline

Xigadee uses a declarative programming model which simplifies the setup of a Microservice within it's container.

## Next: [15 Minute Microservice](fifteenminuteMicroservice.md)

<table><tr> 
<td><a href="http://www.hitachiconsulting.com"><img src="../../docs/hitachi.png" alt="Hitachi Consulting" height="50"/></a></td> 
<td>Created by: <a href="http://github.com/paulstancer">Paul Stancer</a></td>
  <td><a href="https://www.nuget.org/packages/Xigadee">NuGet Package</a></td>
  <td><a href="../../README.md">Home</a></td>
</tr></table>
