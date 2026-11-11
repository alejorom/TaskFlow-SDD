# Proposal

## Why
Actualmente, TaskFlow no dispone de capacidades operativas para la creación de tareas ni de una línea base funcional implementada sobre su arquitectura. Esto impide iniciar el registro de actividades de trabajo y validar de manera práctica el flujo de desarrollo guiado por especificaciones (Spec-Driven Development) sobre la Clean Architecture definida para el proyecto.

La implementación de este cambio resolverá esta necesidad inicial al introducir exclusivamente la capacidad de creación de tareas en TaskFlow, permitiendo registrar nuevas tareas de forma fiable, segura y mantenible mediante un endpoint REST y un caso de uso explícito.

## What Changes
Se introduce de forma focalizada la creación de tareas:
- **Caso de uso `CreateTask`**: Registro de nuevas tareas especificando un título obligatorio, descripción opcional, fecha de vencimiento opcional y prioridad (`Low`, `Medium`, `High`).
- **Estado e identificación inicial**: Asignación automática del estado inicial `Pending` y generación de un identificador único para cada tarea registrada.
- **Endpoint HTTP**: Exposición exclusiva del endpoint `POST /api/tasks` para recibir las solicitudes de creación.
- **Persistencia**: Almacenamiento seguro de las tareas creadas en SQL Server utilizando Entity Framework Core.
- **Validación de entradas y manejo de errores**: Validación de datos de entrada en el dominio/aplicación y respuestas con códigos HTTP estándar (`201 Created`, `400 BadRequest`, `500 InternalServerError`) sin fuga de detalles técnicos o trazas internas.
- **Pruebas automatizadas**: Incorporación de pruebas unitarias y de integración para validar la creación de tareas y los controles asociados.

**Fuera del alcance (exclusiones)**:
- Consulta de tareas individuales o listados.
- Actualización de tareas.
- Eliminación de tareas.
- Filtrado y búsqueda de tareas.
- Modificación o transición manual de estados.
- Autenticación, autorización y control de acceso por roles.
- Asignación de tareas a usuarios, archivos adjuntos y notificaciones.

## Capabilities

### New Capabilities
- `task-creation`: Creación y registro inicial de tareas en el sistema con título obligatorio, descripción opcional, fecha de vencimiento opcional, prioridad y estado inicial predeterminado.

### Modified Capabilities
<!-- Ninguna capacidad existente se modifica en este cambio. -->

## Impact
- **APIs y Contratos**:
  - Exposición únicamente del endpoint `POST /api/tasks`.
  - Documentación del contrato de creación mediante OpenAPI.
- **Estructura de Código y Capas**:
  - `TaskFlow.Domain`: Entidad de tarea, Value Objects/Enums (`TaskStatus`, `TaskPriority`) y reglas invariantes para la creación.
  - `TaskFlow.Application`: Implementación del caso de uso explícito `Tasks/CreateTask` (comando, manejador y DTOs) y abstracción de persistencia, sin intermediarios como MediatR.
  - `TaskFlow.Infrastructure`: Contexto de base de datos de EF Core, mapeo relacional para la entidad de tarea, migraciones para SQL Server e implementación de la persistencia.
  - `TaskFlow.Api`: Endpoint/controlador para `POST /api/tasks`, configuración de inyección de dependencias y middleware global de excepciones.
- **Seguridad y Calidad (OWASP Top 10:2025 y Estándares de Ingeniería)**:
  - *Ámbito de despliegue y control de acceso (A01)*: La API inicial está concebida exclusivamente para un entorno local de desarrollo controlado; no debe exponerse públicamente sin incorporar previamente mecanismos formales de autenticación y autorización.
  - *Validación de entradas (A05, A06)*: Validación estricta de tipos, formatos y longitudes en el payload de creación antes de persistir.
  - *Persistencia segura (A05)*: Consultas parametrizadas gestionadas por EF Core para mitigar riesgos de inyección SQL.
  - *Manejo seguro de excepciones (A10)*: Respuestas de error sin trazas de excepción, cadenas de conexión ni detalles internos.
  - *Estrategia de pruebas*: Pruebas unitarias para las invariantes de dominio y el manejador del caso de uso, junto con pruebas de integración para el endpoint HTTP y la persistencia en base de datos.
