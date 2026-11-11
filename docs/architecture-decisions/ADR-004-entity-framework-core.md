# ADR-004 — Utilizar Entity Framework Core

**Estado:** Accepted  
**Proyecto:** TaskFlow

## Contexto

TaskFlow necesita implementar persistencia sobre SQL Server manteniendo aislada la lógica de negocio de los detalles de acceso a datos.

Se requiere una solución que permita mapear entidades y gestionar cambios en el esquema de base de datos.

## Decisión

Utilizar Entity Framework Core como tecnología ORM.

Su implementación residirá en `TaskFlow.Infrastructure`.

Se utilizarán:

- `TaskFlowDbContext`.
- Configuraciones de entidades mediante Fluent API.
- Migraciones de EF Core.
- Implementaciones de los contratos de persistencia definidos en Application.

## Justificación

- Integración con .NET y SQL Server.
- Mapeo objeto-relacional.
- Soporte para migraciones.
- Facilita la implementación de operaciones de persistencia.

## Consecuencias positivas

- Menor cantidad de código repetitivo de acceso a datos.
- Gestión versionada del esquema.
- Consultas mediante LINQ.
- Configuración explícita de entidades.

## Consecuencias negativas

- Es necesario comprender el comportamiento de las consultas generadas.
- Riesgo de problemas de rendimiento si se utiliza incorrectamente.
- Requiere controlar el seguimiento de entidades y las transacciones.

## Alternativas consideradas

- Dapper.
- ADO.NET.
- SQL escrito manualmente.

## Resultado

Entity Framework Core será la tecnología de persistencia inicial.

Domain y Application no dependerán directamente de EF Core.