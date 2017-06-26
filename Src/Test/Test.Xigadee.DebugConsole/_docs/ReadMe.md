<table>
<tr>
<td width="80%"><a href="../../../README.md"><img src="../../../../docs/X2a.png" alt="Xigadee"></a></td>
<td width = "*" align="right"><img src="../../../../docs/smallWIP.jpg" alt="Sorry, I'm still working here" height="100"></td>
</tr>
</table>

# Test.Xigadee.DebugConsole

This test project is a console application that is used to spin up set of interlinked Microservice applications using the chosen communication protocol, such as Azure Service Bus or direct TCPIP communication.
It serves as a test bed for more complex functionality that can't be easily replicated using standard Unit testing.

To run this application, you will need to pass valid configuration credentials in the Command Line arguments to your own subscription.

## Getting started

This application allows  complex applications to be quickly spun up in a number of different configurations and spread across multiple servers. It allows for the debugging and validations of the complex interactions between Microservices and can be xcopied to multiple machines to simulate client-server environments using different storage and communication options.

## Switches

You can pass all your required settings in the app.config file. However this may be persisted to your source code repository, and you may not want such sensistive information to be stored where is can be seen by anyone with access to your repository.

For this console application, you have the option to pass individual parameters in through the console switches, or pass in a SAS key to a table storage collection which can be shared across multiple developers.

- console.persistence

- console.shortcut
    - startclient
    - startserver
    - startapi

- console.apiuri

- console.slotcount

- console.persistencecache

## Settings

Console options provided without the console will be passed in as override settings to the Microservice application.

<table><tr> 
<td><a href="http://www.hitachiconsulting.com"><img src="../../../../docs/hitachi.png" alt="Hitachi Consulting" height="50"/></a></td> 
<td>Created by: <a href="http://github.com/paulstancer">Paul Stancer</a></td>
  <td><a href="../../../README.md">Home</a></td>
</tr></table>
