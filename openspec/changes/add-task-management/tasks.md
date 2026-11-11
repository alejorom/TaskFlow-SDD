# Tasks: Implementación de CreateTask

## 1. Solución y estructura base de proyectos (.NET 10)

- [ ] 1.1 Crear la solución `TaskFlow.sln` y los cuatro proyectos de Clean Architecture (`TaskFlow.Domain`, `TaskFlow.Application`, `TaskFlow.Infrastructure`, `TaskFlow.Api`) en .NET 10, configurando las referencias de proyecto según las reglas de dependencia (ADR-001) y verificando la compilación con `dotnet build`.
- [ ] 1.2 Crear los proyectos de pruebas automatizadas (`TaskFlow.Domain.UnitTests`, `TaskFlow.Application.UnitTests`, `TaskFlow.IntegrationTests`) con xUnit, referenciando sus respectivos proyectos y verificando la ejecución con `dotnet test`.

## 2. Modelo de Dominio y reglas de negocio

- [ ] 2.1 Implementar las enumeraciones `TaskStatus` y `TaskPriority`, la excepción de dominio `DomainValidationException` y la entidad `TaskItem` en `TaskFlow.Domain` con sus invariantes (título obligatorio de 1 a 200 caracteres, descripción opcional de hasta 2.000 caracteres, `DueDate` tipo `DateOnly?`, `CreatedAt` tipo `DateTimeOffset`, prioridad por defecto `Medium` y estado inicial `Pending`).
- [ ] 2.2 Implementar pruebas unitarias en `TaskFlow.Domain.UnitTests` que verifiquen la creación válida, la asignación de valores por defecto y el lanzamiento de `DomainValidationException` ante entradas inválidas o fechas anteriores a la actual.

## 3. Capa de Aplicación y caso de uso CreateTask

- [ ] 3.1 Definir la abstracción de persistencia `ITaskRepository` (`AddAsync`, `SaveChangesAsync`) en `TaskFlow.Application` y registrar el soporte de `TimeProvider` para referencias temporales en UTC.
- [ ] 3.2 Implementar la feature `Features/Tasks/CreateTask/` con `CreateTaskCommand`, `CreateTaskCommandHandler` y `CreateTaskResponse`, orquestando la creación atómica de la tarea sin intermediarios como MediatR (ADR-005, ADR-006).
- [ ] 3.3 Implementar pruebas unitarias en `TaskFlow.Application.UnitTests` utilizando `FakeTimeProvider` y un doble de prueba de `ITaskRepository` para verificar la orquestación del caso de uso, el manejo de excepciones de dominio y que no haya persistencia ante fallos.

## 4. Infraestructura de persistencia con EF Core y SQL Server

- [ ] 4.1 Configurar la cadena de conexión de forma segura evitando credenciales en archivos versionados mediante variables de entorno o .NET User Secrets (`dotnet user-secrets`), y configurar `TaskFlowDbContext` y el mapeo Fluent API `TaskItemConfiguration` en `TaskFlow.Infrastructure` con conversiones explícitas para almacenar `TaskStatus` y `TaskPriority` como `nvarchar(20)`, columnas `DueDate` como `date` y `CreatedAt` como `datetimeoffset`.
- [ ] 4.2 Implementar el repositorio `TaskRepository` implementando `ITaskRepository` mediante consultas parametrizadas y persistencia atómica con `SaveChangesAsync`.
- [ ] 4.3 Crear la migración inicial de EF Core `InitialCreate_Tasks` acotada a la tabla `Tasks` y verificar la generación correcta de scripts SQL idempotentes sin dependencias de claves foráneas inexistentes.

## 5. Capa API, endpoint POST /api/tasks y Kestrel seguro

- [ ] 5.1 Configurar Kestrel en `TaskFlow.Api` para enlazar exclusivamente a interfaces de loopback (`127.0.0.1` y/o `::1`) en desarrollo, implementando una comprobación *fail-fast* previa a la aceptación de solicitudes que aborte la ejecución (`InvalidOperationException`) si se detecta escucha externa (OWASP A01).
- [ ] 5.2 Implementar el manejador global de excepciones (`IExceptionHandler`) mapeando `DomainValidationException` a HTTP `400 Bad Request` (`ProblemDetails` RFC 9457) y errores imprevistos a HTTP `500 Internal Server Error` sin filtrar trazas ni datos sensibles (OWASP A10).
- [ ] 5.3 Implementar el endpoint `POST /api/tasks` con su DTO de entrada `CreateTaskRequest`, configurando la inyección de dependencias y la documentación OpenAPI sin incluir la cabecera `Location`.

## 6. Pruebas de integración y verificación final

- [ ] 6.1 Implementar pruebas de integración en `TaskFlow.IntegrationTests` con `WebApplicationFactory` para validar el endpoint `POST /api/tasks`, comprobando el código HTTP 201 Created con cuerpo de respuesta, la asignación automática del estado `Pending`, la prioridad predeterminada `Medium`, los límites de longitud (título <= 200, descripción <= 2.000), el rechazo por fecha de vencimiento pasada y la ausencia de cabecera `Location`.
- [ ] 6.2 Implementar pruebas de integración de persistencia utilizando una base de datos SQL Server aislada para pruebas (separada de la base de desarrollo), confirmando el guardado atómico en base de datos tras una creación exitosa y la ausencia de persistencia parcial tras solicitudes inválidas.
- [ ] 6.3 Implementar pruebas de seguridad para verificar la política *fail-fast* de Kestrel ante configuraciones no loopback y la ausencia de trazas internas en respuestas HTTP 500.
- [ ] 6.4 Ejecutar la compilación completa de la solución (`dotnet build`) y la suite completa de pruebas (`dotnet test`), verificando que todos los escenarios de aceptación pasen satisfactoriamente.
