# ADR-006 — Organización de Application por Features

**Estado:** Accepted  
**Proyecto:** TaskFlow  
**Ámbito:** TaskFlow.Application

## Contexto

TaskFlow utilizará Clean Architecture y se desarrollará incrementalmente mediante OpenSpec. La capa Application contendrá los casos de uso que coordinan las operaciones del sistema.

Se evaluaron dos formas de organización:

1. Carpetas globales por tipo técnico: Commands, Queries, Handlers, Validators.
2. Carpetas por funcionalidades de negocio: Features/Tasks/CreateTask, Features/Tasks/GetTasks, etc.

## Decisión

Organizar `TaskFlow.Application` mediante una estructura **Feature-Based**, agrupando cada caso de uso y sus elementos relacionados dentro de su feature.

La estructura inicial será:

`Features/{FeatureName}/{UseCaseName}/`

No se incorporará MediatR inicialmente. Los casos de uso se implementarán mediante clases y contratos explícitos.

## Justificación

- Mayor cohesión funcional.
- Menor dispersión de archivos relacionados.
- Navegación más sencilla.
- Facilita implementar cambios incrementales.
- Permite relacionar casos de uso con especificaciones OpenSpec.
- Facilita delimitar tareas para agentes de programación.

## Consecuencias positivas

- Organización centrada en capacidades de negocio.
- Menor necesidad de modificar carpetas técnicas compartidas.
- Mejor comprensión del alcance de cada funcionalidad.

## Consecuencias negativas

- Posible duplicación de componentes pequeños.
- Riesgo de crear dependencias indebidas entre features.
- Necesidad de criterios claros para extraer componentes compartidos.

## Reglas de cumplimiento

1. Los handlers no deben invocar directamente otros handlers.
2. Las features no deben depender de implementaciones de Infrastructure.
3. Los contratos compartidos deben tener una ubicación explícita.
4. No se deben crear abstracciones compartidas por anticipación.
5. La estructura interna podrá evolucionar mediante nuevos ADR.

## Alternativa descartada

La organización global por tipos técnicos fue descartada por dispersar los elementos de una misma funcionalidad entre diferentes directorios.

## Resultado

Se adopta **Feature-Based Application Organization** como decisión arquitectónica oficial de TaskFlow.