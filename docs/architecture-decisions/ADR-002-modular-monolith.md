# ADR-002 — Adoptar Modular Monolith

**Estado:** Accepted  
**Proyecto:** TaskFlow

## Contexto

TaskFlow comenzará como una aplicación pequeña de gestión de tareas.

Se necesita una estructura que permita evolucionar funcionalmente sin introducir desde el inicio la complejidad operativa de una arquitectura distribuida.

## Decisión

Adoptar un monolito modular (*Modular Monolith*).

La aplicación se desplegará como una única API, con una base de datos SQL Server, y se organizará alrededor de capacidades de negocio.

El módulo inicial será `Tasks`.

## Justificación

- Reduce la complejidad de infraestructura.
- Facilita la depuración y las pruebas.
- Permite agregar nuevas capacidades funcionales de manera organizada.
- Evita introducir comunicación distribuida antes de necesitarla.

## Consecuencias positivas

- Despliegue simplificado.
- Menor costo de mantenimiento.
- Facilidad para realizar transacciones locales.
- Posibilidad de evolucionar los límites modulares.

## Consecuencias negativas

- Los módulos comparten el mismo proceso.
- El despliegue de una funcionalidad implica desplegar la aplicación.
- Se requiere disciplina para evitar acoplamiento entre módulos.

## Alternativas consideradas

- Microservicios.
- Monolito sin organización modular.

## Resultado

TaskFlow utilizará un monolito modular, sin microservicios en su primera versión.

La modularidad inicial será lógica; no se crearán ensamblados independientes por módulo hasta que exista una necesidad justificada.