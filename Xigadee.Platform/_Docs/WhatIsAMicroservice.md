![Work In Progress](../../docs/smallWIP.jpg  "Sorry, I'm still working here")

# What is a Microservice?

Microservices are a relatively new concept that change the way in how we design and build software systems. They are essentially small autonomous services that work together to form an overall solution.

Instead of building your application as one big system, we break it down in to these smaller composable piece, that can be designed, tested and deployed independently from each other. In general Microservices have the following properties:

1. **Complete**
	- A Microservice should only seek to deliver a small - but complete - set of business funtionality; otherwise we will start to build the type of application that we are trying to avoid. 
	A good example of this would be a Customer Microservice that only deals with the interaction with a customer, while another Microservice would handle the purchases for that customer.
	Basically, a Microservice should do one job well, not a multitude of disparate unrelated tasks. In a sense this is similar to the [Single Responsibility Principle](https://en.wikipedia.org/wiki/Single_responsibility_principle) for software development, but on a slightly larger scale.
	A Microservice should have the ability to be independently updated without affecting the rest of the application.	
2. **Scalable and Elastic**
	- Generally when the load on a particular service increases, the technology that implements the service allows it to scale-out to multiple instances to handle this additional load.	
3. **Resilient**
	- Microservices based applications should handle a particular service being unavailable, and should be eventually consistent.
4. **Composable**
	- One of the key benefits of using a Microservice approach, is that it allows for the reuse of the Microservice in other applications or services. We are building a capability that can be used by other services, i.e. a Customer Microservice. How we consume that service can be changed and adjusted over time. We now have a Customer capability, but we are open to integrate that in to other applications as our needs change, without the worry of breaking existing functionality as this service is not tightly coupled in to a specific business funtion.
5. **Minimal**
	- They do one things, and they do it well.


## How is that different from before?

The [Monolith](https://en.wikipedia.org/wiki/Monolithic_application).

## The Gotcha law!

It's important to understand [CAP Theorem](https://en.wikipedia.org/wiki/CAP_theorem) when building a Microservice based application. If you don't them things can get very complicated and messy, very fast. 

## Why use Xigadee?

Xigadee has been built from experience. Many of the problems that we have faced building commercial enterprise-grade Microservice based solutions have been incorporated in to the Xigadee framework. Build these types of application, particularly in a PASS based environment, have particular challenges especially ensuring that your solution is fault tolerant. Xigadee solves many of those challenges for you.

## Next: [Introduction To Xigadee](Introduction.md)

##### Footnotes & Thanks

 - [Mr Fowler](https://martinfowler.com/articles/microservices.html)
 - [Nirmata](http://www.nirmata.com/2015/02/microservices-five-architectural-constraints/)

_@paulstancer_

![Hitachi](../../docs/hitachi.png)
