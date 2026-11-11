# ADR-003 — Utilizar SQL Server

**Estado:** Accepted  
**Proyecto:** TaskFlow

## Contexto

TaskFlow requiere almacenamiento persistente para las tareas y sus propiedades.

La aplicación necesita soportar operaciones de creación, consulta, actualización y eliminación, manteniendo integridad y consistencia de datos.

## Decisión

Utilizar Microsoft SQL Server como sistema de gestión de bases de datos relacional.

El acceso a los datos se implementará en la capa Infrastructure.

## Justificación

- Integración con el ecosistema .NET.
- Soporte de transacciones e integridad referencial.
- Herramientas maduras para administración y diagnóstico.
- Adecuado para el modelo de datos relacional inicial de TaskFlow.

## Consecuencias positivas

- Persistencia transaccional.
- Restricciones de integridad.
- Consultas SQL y capacidades de diagnóstico.
- Integración con Entity Framework Core.

## Consecuencias negativas

- Requiere un servidor o instancia de base de datos.
- Es necesario gestionar configuración, credenciales y migraciones.
- Algunas características de persistencia pueden depender del proveedor.

## Alternativas consideradas

- PostgreSQL.
- SQLite.
- Almacenamiento en memoria.

## Resultado

SQL Server será la base de datos principal de TaskFlow.

Las credenciales no se almacenarán en el repositorio y los entornos de desarrollo y pruebas utilizarán configuraciones independientes.