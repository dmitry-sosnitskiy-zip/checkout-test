
# Payment Gateway

## The task
Build a payment gateway, an API based application that will allow a merchant to offer a way for their shoppers to pay for their product.

## The solution

**Technologies and frameworks used:**
1) ASP.NET Core — provides the means to handle HTTP requests, easily serialize/deserialize data, and perform dependency injection
2) MediarR — implementation of the mediator pattern
2) XUnit, Moq & Shouldly — for unit testing

**Project structure**

The solution uses a Clean Architecture approach ([description](https://www.dandoescode.com/blog/clean-architecture-an-introduction/)) with several layers, each represented by a single C# project:
* `PaymentGateway.Domain` - The Domain Layer holds business model entities and core domain logic for the application
* `PaymentGateway.Application` - The Application Layer contains commands, handlers and validators that represent application 'use cases': they are responsible for input validation and orchestrating domain-level business logic. The Application Layer only references the Domain Layer.
* `PaymentGateway.Infrastructure` - The Infrastructure Layer implements interfaces from the Application Layer to provide functionality to access external systems, such as 3rd-party HTTP APIs and data storage. The Infrastructure Layer references both the Domain and the Application Layers.
* `PaymentGateway.API` - The Presentation Layer contains user-facing API controllers. The Presentation Layer references all other layers.

**Running the project:**
Prequisites: .NET 8 installed, VisualStudio/Rider/other IDE that supports .NET 8
Steps to run:
1) Open `PaymentGateway.sln` in the IDE
2) Set `PaymentGateway.Api` as startup project
3) Launch the project to spin up the development server at `https://localhost:7092/`

**API Endpoints:**  
* `POST https://localhost:7092/payments` - process a payment
* `GET https://localhost:7092/payments/<ID>` - get payment details by ID


Examples:

```
Request:
POST https://localhost:7092/payments
{
  "merchantId": "11111111-1111-1111-1111-111111111111",
  "cardNumber": "2222405343248871",
  "expiryMonth": 10,
  "expiryYear": 30,
  "currency": "USD",
  "amount": 100,
  "cvv": "123"
}


Response:
{
   "id":"1ec6c322-ccef-4ede-8d64-c916c6b7412c",
   "status":"Authorized",
   "lastFourDigits":"8871",
   "expiryMonth":10,
   "expiryYear":30,
   "currency":"USD",
   "amount":100
}

```

```
Request:
https://localhost:7092/payments/1ec6c322-ccef-4ede-8d64-c916c6b7412c

Response:
{
   "id":"1ec6c322-ccef-4ede-8d64-c916c6b7412c",
   "status":"Authorized",
   "lastFourDigits":"8871",
   "expiryMonth":10,
   "expiryYear":30,
   "currency":"USD",
   "amount":100
}
```

**Notes:**
1. The solution allows adding multiple payment gateways (ie acquiring banks) and assigning each merchant to a specific gateway. For the purpose of this exercise I have created three gateways: `MountebankSimulatorGateway`,  `SuccessMockGateway` and `FailMockGateway`. The Mountebank gateway makes HTTP calls to a local Mountebank Simulator, and the other ones respond with hard-coded success/fail statuses.
2. The task description did not make it clear how different types of responses (Authorized, Declined, Rejected) should be returned to the customer, so I chose to use HTTP status codes to represent this distinction (200, 402, and 400 respectively)

**Ideas for improvements:**
With some more work, this solution could be improved in the following ways to make it production-ready:
* Add repository implementation that will connect to a real DB
* Add logging
* Add authentication middleware that would validate the request (for example, read a JWT token from HTTP headers) and set merchant's identity information
* Implement CQRS pattern to support forwarding read and write requests to separate data stores
* Implement idempotency logic to handle concurrent requests
* Pass cancellation tokens to repositories and HTTP clients
* Add DTO-style objects and mappers so that controllers accept and return DTOs instead of directly handling entities from the Application layer
* Add payment audit log to record all events related to each payment
* Improve test coverage
