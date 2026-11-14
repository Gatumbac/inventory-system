# inventory-system

Full-stack product inventory management system built with .NET, Angular, and SQL Server. Everything runs in containers.

## Features

- JWT-based login against users stored in the database
- Product CRUD. The same product can have different prices and stock per supplier or lot number.
- Paginated product listing page
- Responsive UI

## Tech Stack

| Layer    | Technology                                             |
| -------- | ------------------------------------------------------ |
| Frontend | Angular 22                                             |
| Backend  | C# .NET 10, Entity Framework Core                      |
| Database | SQL Server                                             |
| Auth     | JWT                                                    |
| Docs     | Swagger / OpenAPI                                      |
| Logging  | Serilog                                                |
| Runtime  | Docker, Docker Compose                                 |

## Project Structure

```
inventory-system/
├── backend/    # .NET 10 Web API
├── frontend/   # Angular 22 app
└── README.md
```

## Prerequisites

- Docker
- Docker Compose

No local .NET SDK or Node.js installation is required.

## Getting Started

> Compose setup is in progress; these steps will be finalized once it lands.

```bash
cp .env.example .env
docker compose up --build
```

Expected endpoints:

| Service  | URL                           |
| -------- | ----------------------------- |
| Frontend | http://localhost:4200         |
| API      | http://localhost:8080         |
| Swagger  | http://localhost:8080/swagger |

## Configuration

Settings are read from environment variables defined in `.env` (not committed). Use `.env.example` as a template.

## License

For evaluation purposes only.
