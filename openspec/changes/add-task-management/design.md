# Design: Creación de Tareas (CreateTask)

## Context
Actualmente, TaskFlow cuenta con la arquitectura base definida en `docs/architecture.md` y los registros de decisiones arquitectónicas ADR-001 a ADR-006, pero carece de implementación ejecutable en sus capas.

Para motivaciones del negocio, alcance funcional y exclusiones, ver [proposal.md](./proposal.md).  
Para los requisitos de comportamiento, validaciones y escenarios de aceptación observables, ver [spec.md](./specs/task-creation/spec.md).

Este documento establece el diseño técnico, la estructura de componentes, el flujo de dependencias entre capas y las decisiones de ingeniería para implementar de forma atómica y segura el caso de uso `CreateTask`.

## Goals / Non-Goals

**Goals:**
- Implementar la arquitectura Clean Architecture con .NET 10 para soportar el caso de uso `CreateTask` bajo la feature `Tasks`.
- Mantener el modelo de dominio libre de dependencias tecnológicas externas (ADR-001).
- Coordinar la operación mediante un caso de uso explícito sin el uso de MediatR (ADR-005, ADR-006).
- Implementar la persistencia mediante consultas parametrizadas y guardado transaccional con Entity Framework Core sobre SQL Server (ADR-003, ADR-004).
- Asegurar validaciones de dominio y de entrada con defensa en profundidad (OWASP A05, A06).
- Modelar adecuadamente las fechas de calendario (`DateOnly`) frente a marcas temporales (`DateTimeOffset`), asegurando validaciones deterministas vía `TimeProvider`.
- Implementar un manejo global seguro de errores (`ProblemDetails` RFC 9457) diferenciando fallos de validación específicos frente a errores internos (OWASP A10).
- Configurar Kestrel con verificación en tiempo de arranque para fallar (*fail-fast*) si se detecta escucha fuera de loopback en desarrollo (OWASP A01).

**Non-Goals:**
- Implementar operaciones de consulta, actualización, eliminación o listado (reservadas para cambios posteriores).
- Implementar autenticación, autorización o multi-tenancy en esta fase.
- Implementar mecanismos de idempotencia en la API (claves de idempotencia); queda fuera del alcance actual.
- Introducir claves foráneas hacia tablas inexistentes (usuarios, proyectos) en el esquema relacional inicial.
- Modificar las decisiones arquitectónicas fundamentales registradas en los ADR.

## Decisions

### 1. Estructura de capas y asignación de responsabilidades (ADR-001, ADR-002)

Se distribuyen los componentes siguiendo estrictamente la regla de dependencia hacia el interior:

```
[Api] ---> [Application] ---> [Domain]
  |               ^
  +---> [Infrastructure]
```

- **`TaskFlow.Domain`**:
  - `TaskItem`: Entidad raíz que encapsula el identificador, título, descripción, fecha de vencimiento, prioridad, estado y marca de tiempo de creación, garantizando las invariantes en su creación.
  - `TaskStatus`: Enum (`Pending`, `InProgress`, `Completed`, `Cancelled`).
  - `TaskPriority`: Enum (`Low`, `Medium`, `High`).
  - `DomainValidationException`: Excepción específica del dominio lanzada ante incumplimiento de invariantes de negocio.
- **`TaskFlow.Application`**:
  - Organizada por features: `Features/Tasks/CreateTask/`.
  - `CreateTaskCommand`: Registro inmutable con los datos de entrada para la creación.
  - `CreateTaskCommandHandler`: Manejador que orquesta la creación, invocando el dominio y la persistencia.
  - `CreateTaskResponse`: DTO inmutable retornado con los datos de la tarea creada.
  - `ITaskRepository`: Abstracción de persistencia para el agregado de tareas.
  - Inyección de `TimeProvider` para obtener la fecha de referencia actual en UTC.
- **`TaskFlow.Infrastructure`**:
  - `TaskFlowDbContext`: Contexto EF Core configurado para SQL Server.
  - `TaskItemConfiguration`: Configuración Fluent API (`IEntityTypeConfiguration<TaskItem>`) con mapeos de tipos, longitudes y restricciones en SQL Server (columna `date` para `DueDate`, `datetimeoffset` para `CreatedAt`, y `nvarchar(20)` para los enums).
  - `TaskRepository`: Implementación de `ITaskRepository` utilizando `TaskFlowDbContext`.
  - Migración inicial acotada estrictamente a la tabla `Tasks`.
- **`TaskFlow.Api`**:
  - Endpoint `POST /api/tasks` mediante controlador o Minimal API que delega directamente al manejador.
  - `CreateTaskRequest`: Contrato HTTP de entrada.
  - Middleware / `IExceptionHandler`: Transformación controlada de excepciones a respuestas HTTP RFC 9457 (`ProblemDetails`).
  - Configuración y validación en arranque de Kestrel en `Program.cs`.

---

### 2. Orquestación explícita del caso de uso sin MediatR (ADR-005, ADR-006)

El caso de uso `CreateTask` se ejecuta instanciando e inyectando directamente `CreateTaskCommandHandler` en el endpoint de la API.

*Justificación:*
- Elimina la sobrecarga de reflexión y despacho dinámico en tiempo de ejecución.
- Garantiza trazabilidad de código directa, mejor desempeño y verificación de tipos en tiempo de compilación.

---

### 3. Modelo de Dominio, tipos de datos y generación de identificadores

La entidad `TaskItem` genera su propio identificador único `Guid` (`Guid.NewGuid()`) en su instanciación:

- `Id`: `Guid` único no modificable.
- `Title`: `string` no nulo (1 a 200 caracteres).
- `Description`: `string?` opcional (hasta 2.000 caracteres).
- `DueDate`: `DateOnly?` opcional, modelando una fecha de calendario pura sin componente de hora ni zona horaria.
- `Priority`: `TaskPriority` (valor predeterminado `Medium` si no se indica en la solicitud).
- `Status`: `TaskStatus` fijado estrictamente en `Pending` al crearse.
- `CreatedAt`: `DateTimeOffset` con la marca de tiempo de creación provista por el reloj del sistema/`TimeProvider`.

---

### 4. Referencia temporal y validación de fechas (`TimeProvider`)

Se inyecta la abstracción estándar de .NET `TimeProvider` en la capa de aplicación:
- Para obtener la fecha civil actual en UTC:
  ```csharp
  DateOnly todayUtc = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
  ```
- La validación en el dominio comprueba:
  ```csharp
  if (dueDate.HasValue && dueDate.Value < todayUtc)
  {
      throw new DomainValidationException("The due date cannot be in the past.");
  }
  ```

---

### 5. Validación en dos capas (Defensa en profundidad)

1. **Validación de entrada (API / Deserialización)**:
   - Formato JSON correcto, tipos de datos válidos y mapeo de enums.
2. **Validación de reglas e invariantes (Dominio)**:
   - La entidad `TaskItem` valida y rechaza lanzando exclusivamente `DomainValidationException`:
     - Título obligatorio, no vacío, con longitud `1 <= length <= 200`.
     - Descripción con longitud `<= 2000` si está presente.
     - Fecha de vencimiento `>= todayUtc` si está presente.
     - Prioridad perteneciente a los valores definidos en `TaskPriority`.

---

### 6. Persistencia transaccional con EF Core y almacenamiento relacional de Enums (ADR-003, ADR-004)

Se define la interfaz `ITaskRepository` en `TaskFlow.Application`:
```csharp
public interface ITaskRepository
{
    Task AddAsync(TaskItem task, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

- **Almacenamiento de Enums**: Se define de forma definitiva que `TaskStatus` y `TaskPriority` se almacenarán como `nvarchar(20)` mediante conversiones explícitas de EF Core (`HasConversion<string>()` y `.HasMaxLength(20)`). Esto garantiza máxima legibilidad directa en base de datos e independencia ante cambios de orden numérico.
- **Parametrización**: Consultas parametrizadas de forma nativa por EF Core para mitigar riesgos de inyección SQL (OWASP A05).
- **Atomicidad y resultado de guardado**:
  - Las solicitudes inválidas rechazadas por validación no generan ni persisten ningún registro.
  - Toda operación que falle antes de la confirmación (*commit*) no deja persistencia parcial gracias a la transacción gestionada por EF Core.
  - En caso de un fallo de transporte o desconexión de red durante el *commit* en SQL Server, el resultado final puede ser incierto (*uncertain outcome*). Dado que la idempotencia queda fuera del alcance actual, un reintento automático por parte del cliente podría provocar duplicados de la tarea.

---

### 7. Manejo seguro y tipado de errores (OWASP A10)

El middleware de manejo de excepciones (`IExceptionHandler`) gestiona las respuestas de forma diferenciada:
- **`DomainValidationException`**: Se traduce específicamente a HTTP `400 Bad Request` con estructura `ProblemDetails` (RFC 9457), indicando claramente el motivo funcional del rechazo sin revelar detalles internos.
- **Fallos inesperados (incluyendo `ArgumentException` no tipada, `SqlException`, `DbUpdateException`, etc.)**: Se capturan a nivel general, se registran en el sistema de logging con su nivel adecuado y se traducen a HTTP `500 Internal Server Error` con el mensaje genérico seguro: *"An unexpected error occurred while processing your request."*
- **Política de no divulgación**: Bajo ninguna circunstancia se expondrán trazas de pila (*stack traces*), cadenas de conexión, nombres de tablas o detalles internos en respuestas HTTP.

---

### 8. Configuración estricta de Kestrel con fallo en arranque (*Fail-Fast*) (OWASP A01)

Para garantizar que el servicio no escuche accidentalmente fuera del entorno local:
- **Configuración explícita**: En `launchSettings.json` y `Program.cs`, en entorno de desarrollo (`builder.Environment.IsDevelopment()`), Kestrel se enlaza únicamente a `127.0.0.1` y/o `::1`.
- **Comprobación fail-fast antes de aceptar tráfico**:
  - Antes de completar el arranque de la aplicación y comenzar a aceptar solicitudes HTTP, la API inspecciona las direcciones efectivas configuradas (inspeccionando `IServerAddressesFeature` o los endpoints de escucha de Kestrel).
  - Si se detecta cualquier enlace fuera de loopback (como `0.0.0.0`, `IPAddress.Any`, `IPAddress.IPv6Any` o interfaces externas), la aplicación **debe fallar y abortar su inicio inmediatamente**, lanzando una excepción fatal (`InvalidOperationException`) y finalizando el proceso.

---

## Risks / Trade-offs

- **[Ausencia temporal de Autenticación/Autorización]** → *Mitigación:* Enlace forzado a loopback (`127.0.0.1` / `::1`) en Kestrel con comprobación *fail-fast* previa a la aceptación de tráfico; se prohíbe el despliegue en entornos no locales hasta implementar autenticación formal.
- **[Fallo de transporte durante el commit y ausencia de idempotencia]** → *Mitigación:* Si la conexión falla durante el *commit*, el resultado puede ser incierto. Reintentos automáticos a nivel de cliente podrían generar tareas duplicadas. Se documenta explícitamente que la idempotencia está fuera del alcance de este cambio y se incorporará en iteraciones posteriores.
- **[Discrepancias de fecha por zona horaria]** → *Mitigación:* Se utiliza `DateOnly` para `DueDate` y la comparación se efectúa estrictamente contra la fecha del día en UTC (`todayUtc`) obtenida de `TimeProvider`.
- **[Carga de payloads malformados o excesivos]** → *Mitigación:* Límites estrictos de caracteres (200 para título, 2.000 para descripción) e invalidación temprana por parte de Kestrel y del deserializador.

## Migration Plan

1. **Creación de migración inicial acotada (`InitialCreate_Tasks`)**:
   - Generar la migración conteniendo exclusivamente la tabla `Tasks` con las columnas requeridas para `CreateTask`:
     - `Id` (`uniqueidentifier`, PK)
     - `Title` (`nvarchar(200)`, NOT NULL)
     - `Description` (`nvarchar(2000)`, NULL)
     - `DueDate` (`date`, NULL)
     - `Priority` (`nvarchar(20)`, NOT NULL, vía conversión de enum)
     - `Status` (`nvarchar(20)`, NOT NULL, vía conversión de enum)
     - `CreatedAt` (`datetimeoffset`, NOT NULL)
   - No incluir claves foráneas hacia tablas inexistentes.
2. **Aplicación controlada**:
   - Ejecutar la migración mediante `dotnet ef database update` en el entorno de desarrollo local.
3. **Estrategia y riesgo de Rollback**:
   - El rollback se ejecuta mediante `dotnet ef database update 0` o ejecutando el método `Down()` de la migración.
   - *Advertencia de riesgo:* El rollback es una **operación controlada y destructiva** que eliminará por completo la tabla `Tasks` y cualquier dato almacenado en ella.

## Open Questions

- *Estrategia de despliegue de migraciones en CI/CD*: Se formalizará si se emplearán bundles ejecutables (`efbundle`) o scripts SQL idempotentes cuando se implemente el pipeline de despliegue continuo.
