# ADR-005 — Casos de uso explícitos sin MediatR

**Estado:** Accepted  
**Proyecto:** TaskFlow

## Contexto

TaskFlow necesita coordinar operaciones de negocio mediante casos de uso identificables, manteniendo responsabilidades claras.

El proyecto también tiene un objetivo educativo: comprender directamente cómo se implementan y ejecutan los casos de uso.

## Decisión

Implementar los casos de uso mediante clases C# explícitas, organizadas dentro de las features correspondientes.

Se utilizarán Commands y Queries para representar las solicitudes de escritura y lectura cuando resulte apropiado.

Cada caso de uso tendrá un Handler responsable de coordinar su ejecución.

No se incorporará MediatR inicialmente.

## Justificación

- Permite comprender el flujo de ejecución sin intermediarios adicionales.
- Reduce dependencias externas.
- Facilita la depuración.
- Hace explícitas las dependencias de cada Handler.
- Mantiene abierta la posibilidad de introducir MediatR posteriormente.

## Consecuencias positivas

- Flujo de ejecución transparente.
- Menor complejidad inicial.
- Casos de uso fáciles de localizar y probar.
- Inyección de dependencias explícita.

## Consecuencias negativas

- Es necesario implementar manualmente la invocación de los handlers.
- No se dispone automáticamente de un pipeline de comportamientos.
- Algunas responsabilidades transversales pueden requerir mecanismos adicionales.

## Alternativas consideradas

- MediatR.
- Lógica de aplicación directamente en endpoints.
- Servicios de aplicación grandes con múltiples operaciones.

## Relación con ADR-006

ADR-005 define **cómo se implementan los casos de uso**.

ADR-006 define **cómo se organizan los archivos de esos casos de uso**.

Ambas decisiones son complementarias.

## Resultado

TaskFlow utilizará casos de uso explícitos mediante clases y handlers ordinarios, sin MediatR inicialmente.