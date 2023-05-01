# MiniERP

### Table of content

1. [Project Description](#project-description)
2. [Architecture](#architecture)
3. [Technologies](#technologies)

## Project Description

MiniERP is an ever-evolving back-end for a fictional ERP system. It stems from an attempt to reconcile my current job as an ERP integration developper with a desire to explore Software Architecture and Distributed Systems. Having been in contact with how massive ERPs can be, with lots of related components, I felt that it was the perfect canvas to try new technologies. Just add new components! 

The base is composed of 4 modules : 
* Product Service
* Inventory Service
* Purchase Order service
* Sales Order Service
  
 Those four modules allowed me to explore service decoupling via a Message Bus, for which the architecture was the hardest part : how can I make these services autonomous, while still being interdependant? SOs and POs need to validate the inventory before being created. A cache was used from which they have read access, while write access is given to the Inventory service. RPC call is made in case the cache is unavailable.
 
 All in all, this weas a very fun project, that allowed me to get acquainted with modern Distribute System technologies and patterns. This is obviously not a real-file application, but more an exploration into something bigger.I plan on returning back to it and make it grow with time.
 
 ## Architecture
 
![Screenshot](Diagram.png)

The architecture of MiniERP is designed to be scalable, resilient, and loosely coupled. It consists of four primary modules, each responsible for specific functions:

* Product Service: Handles the creation of new products
* Inventory Service: Manages stock levels and provides real-time inventory updates
* Purchase Order Service: Creates purchase orders to replenish inventory
* Sales Order Service: Processes sales orders to sell products

These modules are implemented as ASP.NET Web API services, each with its own PostgreSQL database. They are hosted as pods on a single Kubernetes cluster to reduce costs, and an NGINX controller is used as a reverse proxy to route HTTPS requests from outside the cluster to the services inside.

In order to provide fast access to inventory data, a Redis cache is used. The Inventory Service has write access to the cache, while the SO and PO Services have read access. If the cache is unavailable, the services fall back to making gRPC calls to the Inventory Service.

To ensure loose coupling between the services, a RabbitMQ message bus is used for inter-service communication. The Inventory Service subscribes to order-related messages and updates inventory levels accordingly. This architecture allows each module to be autonomous while still being interdependent, enabling easier maintenance and scalability.

Finally, to handle requests and validation within each service, the CQRS pattern is used with MediatR, and FluentValidation is used for validating input items. Retry strategies are also implemented using Polly, to provide resilience in the case of cache or gRPC failures.

## Technologies

* Kubernetes/Docker
* .NET 6
* Redis
* RabbitMQ
* PosgreSQL
* gRPC
