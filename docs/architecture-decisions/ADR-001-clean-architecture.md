# ADR-001 — Adoptar Clean Architecture

**Estado:** Accepted  
**Proyecto:** TaskFlow

## Contexto

TaskFlow será una API REST desarrollada en .NET 10, utilizada como laboratorio para aprender SDD, OpenSpec y desarrollo asistido por agentes de IA.

Se requiere una arquitectura que permita separar las reglas de negocio de los detalles tecnológicos, mantener el código verificable y facilitar cambios incrementales.

## Decisión

Adoptar Clean Architecture, organizando la solución en cuatro proyectos:

- `TaskFlow.Domain`
- `TaskFlow.Application`
- `TaskFlow.Infrastructure`
- `TaskFlow.Api`

Las dependencias entre proyectos seguirán la regla de dependencia hacia las capas internas.

## Justificación

- Separación de responsabilidades.
- Independencia de las reglas de negocio respecto de la persistencia y HTTP.
- Facilidad para implementar pruebas automatizadas.
- Mayor control sobre las implementaciones generadas por agentes de IA.
- Posibilidad de sustituir componentes tecnológicos sin modificar el núcleo de negocio.

## Consecuencias positivas

- Límites arquitectónicos explícitos.
- Mayor facilidad para realizar pruebas unitarias.
- Menor acoplamiento entre negocio e infraestructura.

## Consecuencias negativas

- Mayor número de proyectos y archivos.
- Necesidad de configurar Dependency Injection.
- Riesgo de introducir abstracciones innecesarias.

## Alternativas consideradas

- Arquitectura tradicional en capas.
- Arquitectura monolítica con un único proyecto.
- Vertical Slice Architecture sin separación formal de proyectos.

## Resultado

Se adopta Clean Architecture como estructura arquitectónica principal de TaskFlow.