![Work In Progress](../../docs/smallWIP.jpg  "Sorry, I'm still working here")

# What is a Microservice?

Microservices are a relatively new concept that change the way in how way we design and build software systems. They are essentially small autonomous services that work together to form an overall solution.

Instead of building your application as one big system, we break it down in to these smaller composable pieces called Microservices. In general Microservices have the following properties:

1. **Complete**
	- A Microservice should only seek to deliver a small complete set of business funtionality; otherwise we will start to build the type of application that we are trying to avoid. 
	A good example of this would be a Customer Microservice that only deals with the interaction with a customer, while another Microservice would handle the purchases for that customer.
	Basically, a Microservice should do one job well, not a multitude of disparate unrelated tasks. In a sense this is similar to the [Single Responsibility Principle](https://en.wikipedia.org/wiki/Single_responsibility_principle) for software development, but on a slightly larger scale.
	
2. **Scalable and Elastic**
	- Generally when the load on a particular service increases, the technology that implements the service allows it to scale-out to multiple instances to handle this additional load.
	
3. **Resilient**
	- Microservices based applications should handle a particular service being unavailable, and should be eventually consistent.
4. **Composable**
5. **Minimal**
	- They do one things, and they do it well.


## How is that different from before?

## The Gotcha law!

It's important to understand [CAP Theorem](https://en.wikipedia.org/wiki/CAP_theorem) when building a Microservice based application. If you don't them things can get very complicated and messy, very fast. 

## How does Xigadee work?

## Next: [Introduction To Xigadee](Introduction.md)

#### Footnotes

 - [Mr Fowler](https://martinfowler.com/articles/microservices.html)
 - [Nirmata](http://www.nirmata.com/2015/02/microservices-five-architectural-constraints/)

_@paulstancer_

![Hitachi](../../docs/hitachi.png)
