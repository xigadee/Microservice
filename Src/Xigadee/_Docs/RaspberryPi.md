![Xigadee](../../../docs/X2a.png)

# Xigadee on Linux and the Raspberry Pi

This section is a guide to setting up Xigadee on ARM based Linux distros such as the Raspberry Pi. Raspberry Pi comes with two many types of Linux. For this post I have tested it using two specific distros: [Raspbian](https://www.raspberrypi.org/downloads/raspbian/) and [Ubuntu Mate](https://ubuntu-mate.org/raspberry-pi/), and this post will explain the differences and how to set up .NET Core on both.

You can also use this as a guide to setting up Xigadee apps to run on standard x64, x86 based Linux environments.

## Platform
Currently .NET Core supports ARM based processors in [preview](https://github.com/dotnet/announcements/issues/29) that support ARM7.1 or higher. This means that the framework can be run on the Raspberry Pi 2 & 3, but I have not tested on the Pi Zero, and I believe it will not work due to the use of an older ARM6 architecture.

## Getting started


<table><tr> 
  <td>Created by: <a href="http://github.com/paulstancer">Paul Stancer</a></td>
  <td><a href="../../../README.md">Home</a></td>
</tr></table>