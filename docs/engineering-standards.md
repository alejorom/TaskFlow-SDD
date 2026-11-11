# TaskFlow — Engineering Standards

**Estado:** Accepted  
**Ámbito:** Todo el repositorio  
**Referencias:** Clean Architecture, OWASP Top 10:2025, principios SOLID

## 1. Principios generales

Toda implementación deberá:

- Respetar las decisiones arquitectónicas vigentes.
- Aplicar principios SOLID cuando correspondan.
- Mantener responsabilidades claramente separadas.
- Priorizar simplicidad, legibilidad y mantenibilidad.
- Evitar duplicación innecesaria y sobreingeniería.
- Utilizar las capacidades seguras y soportadas de .NET.
- Incluir pruebas relevantes para el comportamiento implementado.

## 2. Seguridad por diseño

La seguridad deberá considerarse desde la especificación y el diseño, no únicamente al finalizar la implementación.

Cada cambio deberá evaluar los riesgos aplicables de OWASP Top 10:2025.

Los controles deberán ser proporcionales al riesgo y al alcance de la funcionalidad.

## 3. Controles de seguridad

### Validación de entradas

- Validar tipos, formatos, longitudes y restricciones de negocio.
- Rechazar entradas inválidas.
- No confiar en los datos recibidos del cliente.

### Control de acceso

- Aplicar autorización del lado del servidor cuando existan identidades y permisos.
- Utilizar el principio de mínimo privilegio.
- No considerar segura una operación únicamente porque su interfaz esté oculta.

### Persistencia

- Utilizar consultas parametrizadas.
- Evitar concatenar entradas no confiables en SQL.
- Mantener credenciales fuera del código fuente.

### Manejo de errores

- No exponer detalles internos, cadenas de conexión ni trazas de excepción a los clientes.
- Utilizar respuestas HTTP consistentes.
- Registrar información diagnóstica sin revelar datos sensibles.

### Configuración y dependencias

- Mantener dependencias actualizadas y revisar vulnerabilidades conocidas.
- Evitar paquetes innecesarios.
- Separar configuraciones de desarrollo y producción.
- No publicar secretos en Git.

### Logging

- Registrar eventos relevantes para diagnóstico y seguridad.
- No registrar contraseñas, tokens ni información sensible innecesaria.
- Utilizar niveles de logging adecuados.

## 4. OWASP Top 10:2025

Toda funcionalidad deberá evaluar las categorías aplicables:

1. A01 — Broken Access Control.
2. A02 — Security Misconfiguration.
3. A03 — Software Supply Chain Failures.
4. A04 — Cryptographic Failures.
5. A05 — Injection.
6. A06 — Insecure Design.
7. A07 — Authentication Failures.
8. A08 — Software or Data Integrity Failures.
9. A09 — Security Logging and Alerting Failures.
10. A10 — Mishandling of Exceptional Conditions.

## 5. Calidad del código

- Utilizar nombres descriptivos.
- Mantener métodos y clases con responsabilidades claras.
- Evitar dependencias circulares.
- Aplicar Dependency Injection.
- Evitar lógica de negocio en controllers o endpoints.
- No introducir frameworks sin justificación.
- Documentar decisiones arquitectónicas significativas.

## 6. Estrategia de pruebas

Los cambios deberán incluir pruebas proporcionales a su alcance:

- Pruebas unitarias para reglas de dominio y casos de uso.
- Pruebas de integración para persistencia y endpoints.
- Pruebas de validación de entradas.
- Pruebas de seguridad para controles aplicables.
- Pruebas de regresión cuando se corrijan defectos.

## 7. Criterios de finalización

Una funcionalidad no se considerará terminada hasta que:

- Cumpla las especificaciones aprobadas.
- Respete la arquitectura.
- Se hayan revisado los riesgos de seguridad aplicables.
- Las pruebas pertinentes se ejecuten satisfactoriamente.
- No existan secretos incorporados al repositorio.
- Los errores y limitaciones conocidos estén documentados.

## 8. Referencias

- OWASP Top 10:2025: https://top10.owasp.org/2025/
- Documentación de arquitectura: `docs/architecture.md`
- Decisiones arquitectónicas: `docs/architecture-decisions/`