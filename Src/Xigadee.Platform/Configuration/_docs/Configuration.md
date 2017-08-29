<table>
<tr>
<td width="80%"><a href="../../../README.md"><img src="../../../../docs/X2a.png" alt="Xigadee"></a></td>
<td width = "*" align="right"><img src="../../../../docs/smallWIP.jpg" alt="Sorry, I'm still working here" height="100"></td>
</tr>
</table>

# Configuration

Xigadee configuration is built to be flexible and to support multiple configuration layers from different mechanisms, and to prioritise them by setting a priority to the specific configuration level.

At the root of the Xigadee configuration stack is the usual app.config or web.config file. 
This is what most apps will use by default, but this can be extended to support mechanisms such as Azure KeyVault, 
Table storage, SQL databases.

When multiple keys are resolved from multiple storage repositories, the priority of the mechanisms can be set to ensure only one key is returned.

You can also pass keys as console parameters which can be useful when creating simple container applications.

- Config file (app.config / web.config)

- Manual Settings 

- Azure Table Storage

- Azure Key Vault

- Custom container



<table><tr> 
<td><a href="http://www.hitachiconsulting.com"><img src="../../../../docs/hitachi.png" alt="Hitachi Consulting" height="50"/></a></td> 
<td>Created by: <a href="http://github.com/paulstancer">Paul Stancer</a></td>
  <td><a href="https://www.nuget.org/packages/Xigadee">NuGet Package</a></td>
  <td><a href="../../../README.md">Home</a></td>
</tr></table>
