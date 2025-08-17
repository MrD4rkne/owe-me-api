# OweMe API Documentation

[![Build release](https://github.com/MrD4rkne/owe-me-api/actions/workflows/build-release.yml/badge.svg)](https://github.com/MrD4rkne/owe-me-api/actions/workflows/build-release.yml)
[![Daily tests](https://github.com/MrD4rkne/owe-me-api/actions/workflows/build-regular.yml/badge.svg)](https://github.com/MrD4rkne/owe-me-api/actions/workflows/build-regular.yml)
[![Run Smoke Tests](https://github.com/MrD4rkne/owe-me-api/actions/workflows/run-smoketests.yml/badge.svg)](https://github.com/MrD4rkne/owe-me-api/actions/workflows/run-smoketests.yml)

## Overview

OweMe API is a .NET-based web API built with Clean Architecture principles. The application manages financial tracking and ledger operations with a focus on maintainability, testability, and scalability.

## Project Structure

The solution follows Clean Architecture patterns with clear separation of concerns:

```
OweMe.Api/
├── api/src/                    # Main application source code
│   ├── OweMe.Api/             # 🎯 Presentation Layer - Web API endpoints
│   ├── OweMe.Application/     # 📋 Application Layer - Business logic & use cases
│   ├── OweMe.Domain/          # 🏛️ Domain Layer - Core business entities
│   ├── OweMe.Infrastructure/  # 🔧 Infrastructure Layer - External concerns
│   └── OweMe.Persistence/     # 💾 Data Access Layer - Database operations
├── api/tests/                 # Test projects
├── client/                    # Client-side code generation
└── docs/                      # Documentation
```

## Technologies

- **.NET 9.0**: Core framework for building the API.
- **PostgreSQL**: Database for data persistence.
- **Docker**: Containerization for development and deployment.
- **GitHub Actions**: CI/CD pipeline for automated builds and tests.
- **OpenApi 3.0, Scalar, NSWag**: API documentation and client code generation.
- **Wolverine**: as mediator (and in the future as messaging framework).

## Development

### Prerequisites
- .NET 9.0 SDK
- Docker (for local development)
- PostgreSQL (via Docker Compose)

### Local Setup
```bash
# Clone and navigate to the project
git clone https://github.com/MrD4rkne/owe-me-api
cd owe-me-api

# Start
docker-compose -f api/src/compose.yaml up -d
```

## CI/CD

Project has CI/CD flow for easier development and deployment. See more in
[CI/CD readme](./docs/ci-cd.MD).

## Tests

How I try to verify stuff works :)

[Tests readme](./docs/tests.MD)

## License

See [License file](./LICENSE)