# ECommerce Microservices with Docker Compose

## Overview

This project demonstrates an ECommerce application with three microservices: Order, Inventory, and Notification Services, using SQL Server and RabbitMQ, orchestrated with Docker Compose.

## Services Overview

### Order Service

- Receives order requests with order ID, product ID, and quantity
- Validates order and publishes `OrderCreated` event to RabbitMQ

### Inventory Service

- Listens for `OrderCreated` events
- Checks stock availability:
  - If in stock: Reduces inventory, publishes `InventoryUpdated` event
  - If out of stock: Publishes `OutOfStock` event

### Notification Service

- Listens for `InventoryUpdated` and `OutOfStock` events
- Sends mock notifications about order processing status

### SQL Server (MSSQL)

- **Credentials**:
  - Username: `sa`
  - Password: `TestPassword123!`
- Provides database for Order and Inventory services
- Stores order and inventory data

### RabbitMQ

- Message broker handling events between services
- Accessible via management UI at `http://localhost:15672`
- Services communicate using `rabbitmq` hostname

## Prerequisites

- Docker and Docker Compose
- .NET SDK 8.0
- Basic Docker, RabbitMQ, and SQL Server knowledge

## Project Setup

### 1. Clone Repository

```bash
git clone [repository link](https://github.com/Hassan-Ayman-SE/ECommerce.git)
cd ECommerce
```

### 2. Service Configurations

- **Order Service**: Port 5000, connects to RabbitMQ and SQL Server
- **Inventory Service**: Port 5001, processes inventory events
- **Notification Service**: Port 5002, logs notifications
- **SQL Server**: Port 1433
- **RabbitMQ**: Ports 5672 (service), 15672 (management UI)

### 3. Running Application

```bash
# Start services
docker-compose up --build

# Stop services
docker-compose down

# Remove volumes
docker-compose down -v
```

## Service URLs

- Order Service: http://localhost:5000
- Inventory Service: http://localhost:5001
- Notification Service: http://localhost:5002
- RabbitMQ UI: http://localhost:15672 (user: guest, password: guest)

## Communication Flow

1. Order Service sends `OrderCreated` event
2. Inventory Service processes event:
   - Reduces stock if available
   - Sends `InventoryUpdated` or `OutOfStock`
3. Notification Service logs order status

## Key Points

- RabbitMQ exposed on port 5672
- Services depend on RabbitMQ and MSSQL
- `RABBITMQ_HOST` set to RabbitMQ container name
- Localhost config enables containerized service communication

## Troubleshooting

- **Service Not Starting**: Check Docker resources
- **RabbitMQ Issues**: Verify `RABBITMQ_HOST` and RabbitMQ status
- **SQL Server Problems**: Confirm initialization on localhost:1433
