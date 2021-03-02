# CTU Basic Microservices Demo

## Scope

This is meant to be a very basic demonstration of communication between muliple services via a message bus like RabbitMQ.

Given a user wants to add an item to a cart and be notified if the item is in stock. Use the microservices pattern to create a cart, product and orders services with a message bus as communicaiton between them.

## Microservice
### Cart 
Should expose a POST Http endpoint which accepts a CartItemDto and publishes the dto to the "cart-item-added" queue for order to consume from.

### Orders
A services which should subscribe to the "cart-item-added" queue which executes a the create OrderCreateHandler which will save the request to a DB and to the "order-placed" queue for the product service to consume from.

### Product

This service should expose a RESTful Get enpoint to retrieve a basic list of products. 

Create a handler function to increase or decrease a product quantity and publish an events to either 
a) Publish a message to the "stock-confirmed" queue if the qty > 0
b) Publish a message to the "stock-shortage" queue if the qty == 0 

This function should be called from the OrderPlacedSubscriber which consumes message from the "order-placed" queue.

## Diagram

![diagram](http://url/to/img.png)

## Prerequisites 

- Docker
- Dotnet Core 3.1
- Visual Studio or VSCode

## Getting started

 Navigate to the route directory where the docker-compose.yml file is located. Open up a new console window/terminal and run 

 ```
docker-compose up -d
 ```

Once the RabbitMQ nad SQL Server started navigate to each project with a db context and run the migrations which will create and seed the relevant DBs. To access the each microservice's databse connect to it with [YourIPAddress],1433 and credentials for local host can be found in the docker-compose.yml file.

You can access the RabbitMQ Dashboard at http://localhost:15672/ U:guest P:guest

## Running the projects

Startup order Cart, Order, Products

HTTP Requests:
```
GET: https://localhost:44325/products
RESULT: [
    {
        "id": 1,
        "name": "Alienware M15 R3",
        "qty": 5
    },
    {
        "id": 2,
        "name": "Asus Zephyrus G14",
        "qty": 0
    },
    {
        "id": 3,
        "name": "Razer Blade 17 Pro",
        "qty": 0
    }
]
```
```
POST: https://localhost:44355/cart
BODYTYPE: JSON
BODY: {
    "ProductId": int,
    "Qty": int
}
```
