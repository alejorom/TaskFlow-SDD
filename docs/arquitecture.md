# TaskFlow — Architecture Document

**Version:** 1.0  
**Status:** Proposed  
**Framework:** .NET 10  
**Architecture Style:** Clean Architecture  
**Deployment Model:** Modular Monolith

## 1. Purpose

This document defines the architectural structure, technical decisions, component responsibilities, dependency rules, and development conventions for the TaskFlow application.

It serves as the architectural reference for developers and AI coding agents working on the project.

All implementations must comply with the architectural principles established in this document.

## 2. Project Overview

TaskFlow is a task management application developed as a learning project to demonstrate Spec-Driven Development (SDD), Clean Architecture, and AI-assisted software engineering.

The initial application will expose a REST API that allows users to create, retrieve, update, delete, filter, and manage tasks.

### 2.1 Initial Functional Scope

- Create tasks.
- Retrieve tasks.
- Retrieve individual tasks.
- Update tasks.
- Delete tasks.
- Manage task status.
- Manage task priority.
- Filter tasks.

Authentication, authorization, notifications, and multi-tenancy are outside the initial scope.

## 3. Architectural Style

### 3.1 Clean Architecture

TaskFlow will follow Clean Architecture principles.

The application will be divided into four projects:

- TaskFlow.Domain
- TaskFlow.Application
- TaskFlow.Infrastructure
- TaskFlow.Api

The primary architectural principle is dependency inversion.

Business rules must not depend on infrastructure or external frameworks.

### 3.2 Modular Monolith

TaskFlow will initially be implemented as a modular monolith.

The application will have a single deployment unit and a single API, while maintaining clear internal separation of responsibilities.

Microservices will not be introduced during the initial development phase.

## 4. Technology Stack

| Component | Technology |
|---|---|
| Runtime | .NET 10 |
| Backend | ASP.NET Core Web API |
| Language | C# |
| Database | Microsoft SQL Server |
| ORM | Entity Framework Core |
| API Contract | OpenAPI |
| Unit Testing | xUnit |
| Integration Testing | xUnit |
| Version Control | Git |
| Requirements Management | OpenSpec |

Compatible package versions must be selected for .NET 10.

## 5. Project Responsibilities

### 5.1 TaskFlow.Domain

Contains the core business model and domain rules.

Responsibilities:

- Entities.
- Value Objects, when necessary.
- Enumerations.
- Domain validation.
- Business invariants.

Restrictions:

- Must not depend on other TaskFlow projects.
- Must not depend on Entity Framework Core.
- Must not depend on ASP.NET Core.
- Must not contain database access logic.

### 5.2 TaskFlow.Application

Contains application use cases and coordinates business operations.

Responsibilities:

- Commands and Queries.
- Use Case Handlers.
- Application interfaces.
- Application-level validation.
- Application DTOs.
- Dependency abstractions.

Use cases will be organized by feature and operation.

Initial organization:

- Tasks/CreateTask
- Tasks/GetTasks
- Tasks/GetTaskById
- Tasks/UpdateTask
- Tasks/DeleteTask

The application layer will define the abstractions required to access persistence.

Restrictions:

- Must depend only on Domain and approved application-level abstractions.
- Must not depend on Infrastructure.
- Must not directly access SQL Server.
- Must not contain HTTP-specific concerns.

MediatR will not be introduced initially.

### 5.3 TaskFlow.Infrastructure

Contains implementations of external technical dependencies.

Responsibilities:

- Entity Framework Core configuration.
- Database context.
- Repository implementations, when justified.
- SQL Server connectivity.
- Database migrations.
- External service implementations, when needed.

Infrastructure implements the interfaces defined by Application.

### 5.4 TaskFlow.Api

Provides the HTTP interface to the application.

Responsibilities:

- REST endpoints.
- Request and response handling.
- Dependency injection configuration.
- HTTP status code mapping.
- Middleware.
- Exception handling.
- OpenAPI configuration.

The API layer must delegate business operations to Application use cases.

Business rules must not be implemented inside controllers or endpoints.

## 6. Dependency Rules

The dependency direction must always point inward.

Allowed project references:

| Project | Allowed references |
|---|---|
| Domain | None |
| Application | Domain |
| Infrastructure | Application, Domain |
| Api | Application, Infrastructure |

Infrastructure implements Application abstractions.

The Domain layer remains independent of external technologies.

Dependency injection will be configured at the composition root in the API project.

## 7. Application Organization

Application will follow a use-case-oriented structure.

Each use case must have a clear and specific responsibility.

Commands represent operations that modify application state.

Queries represent operations that retrieve information.

Handlers coordinate the execution of their corresponding use cases.

MediatR and other mediator frameworks are not required for the initial implementation.

## 8. Persistence Strategy

SQL Server will be the primary relational database.

Entity Framework Core will be used as the Object-Relational Mapper.

Database access must be isolated within Infrastructure.

Application must interact with persistence through abstractions rather than directly depending on EF Core.

Database migrations must be version-controlled.

## 9. API Design

TaskFlow will expose a REST API.

The API contract will be documented using OpenAPI.

Initial resource:

`/api/tasks`

Initial operations:

- GET /api/tasks
- GET /api/tasks/{id}
- POST /api/tasks
- PUT /api/tasks/{id}
- DELETE /api/tasks/{id}

HTTP status codes and response models must follow REST conventions.

The OpenAPI contract must remain consistent with the implemented endpoints.

## 10. Testing Strategy

### Unit Tests

Unit tests will validate:

- Domain business rules.
- Application use cases.
- Validation behavior.
- Expected error conditions.

Unit tests must not require a real SQL Server instance.

### Integration Tests

Integration tests will validate:

- HTTP endpoints.
- Application and Infrastructure integration.
- Database persistence.
- Dependency injection configuration.

Integration tests must verify relevant behavior using a controlled database environment.

## 11. Cross-Cutting Concerns

The initial implementation must consider:

- Structured logging.
- Centralized exception handling.
- Configuration management.
- Dependency injection.
- Input validation.
- Consistent HTTP responses.

Authentication, authorization, caching, and observability enhancements may be introduced in future changes.

## 12. Architecture Constraints

The following rules are mandatory:

1. Domain must remain independent of infrastructure.
2. Application must not reference Infrastructure.
3. API endpoints must not contain business logic.
4. Database access must remain isolated in Infrastructure.
5. Use cases must have explicit responsibilities.
6. Business rules must be testable independently.
7. External dependencies must be introduced only when justified.
8. Changes to architectural rules must be documented and reviewed.

## 13. Evolution Strategy

The architecture will evolve incrementally.

New capabilities must be introduced through OpenSpec changes.

Each change must include its functional specifications and, when appropriate, its technical design.

Architectural decisions that affect the overall system must be documented through Architecture Decision Records (ADRs).

The architecture must be reviewed whenever a proposed change introduces new dependencies, modifies layer responsibilities, or changes the deployment model.