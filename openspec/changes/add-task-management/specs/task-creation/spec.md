# Spec Delta

## Purpose
Permite a los usuarios y sistemas clientes registrar y crear nuevas tareas de trabajo en TaskFlow, asegurando la integridad de los datos de entrada, asignando automáticamente su estado inicial y persistiendo la información de forma segura.

## ADDED Requirements

### Requirement: Registro de nueva tarea con datos válidos
El sistema DEBERÁ (SHALL) permitir la creación de una tarea mediante `POST /api/tasks` cuando la solicitud contenga un título válido, asignando un identificador único generado por el sistema y retornando código HTTP 201 Created con los datos de la tarea en el cuerpo de la respuesta, sin incluir cabecera Location.

#### Scenario: Creación exitosa de tarea con campos obligatorios y opcionales
- **WHEN** el cliente envía una solicitud `POST /api/tasks` con un título válido "Completar reporte", descripción de hasta 2.000 caracteres, fecha de vencimiento futura o actual y prioridad "High"
- **THEN** el sistema crea la tarea, genera un identificador único, responde con código HTTP 201 Created y devuelve en el cuerpo la tarea con todos sus atributos, sin incluir cabecera `Location`

#### Scenario: Creación exitosa de tarea únicamente con campos obligatorios
- **WHEN** el cliente envía una solicitud `POST /api/tasks` con un título válido "Planificar sprint", omitiendo descripción y fecha de vencimiento
- **THEN** el sistema crea la tarea con los valores opcionales en nulo o vacíos, genera su identificador único y responde con código HTTP 201 Created con la tarea en el cuerpo de la respuesta

### Requirement: Asignación automática de estado inicial y prioridad
El sistema DEBERÁ (SHALL) establecer automáticamente el estado de la tarea en `Pending` al momento de su creación y registrar la prioridad especificada (`Low`, `Medium` o `High`). Si la prioridad se omite en la solicitud, el sistema DEBERÁ (SHALL) asignar `Medium` como prioridad predeterminada.

#### Scenario: Asignación de estado inicial Pending y prioridad explícita
- **WHEN** el cliente crea una tarea especificando prioridad "Low"
- **THEN** el sistema asigna el estado `Pending` y la prioridad `Low` a la tarea recién creada

#### Scenario: Asignación de prioridad predeterminada Medium cuando se omite
- **WHEN** el cliente envía una solicitud de creación sin especificar el campo de prioridad
- **THEN** el sistema asigna automáticamente el estado `Pending` y la prioridad predeterminada `Medium` a la tarea

### Requirement: Validación estricta de entradas y rechazo de datos inválidos
El sistema DEBERÁ (SHALL) validar la solicitud, exigiendo título obligatorio no vacío de hasta 200 caracteres, descripción opcional de hasta 2.000 caracteres, fecha de vencimiento opcional válida igual o posterior a la actual y prioridad válida. Toda solicitud inválida DEBERÁ (SHALL) rechazarse con HTTP 400 Bad Request sin persistir la tarea.

#### Scenario: Rechazo por título vacío o ausente
- **WHEN** el cliente envía una solicitud `POST /api/tasks` con el campo título vacío, compuesto solo por espacios o ausente
- **THEN** el sistema rechaza la solicitud con código HTTP 400 Bad Request, devuelve un mensaje descriptivo y no genera ni persiste ninguna tarea

#### Scenario: Rechazo por título que excede la longitud máxima
- **WHEN** el cliente envía una solicitud `POST /api/tasks` con un título que supera los 200 caracteres
- **THEN** el sistema rechaza la solicitud con código HTTP 400 Bad Request indicando que la longitud máxima ha sido superada y no persiste ninguna tarea

#### Scenario: Rechazo por descripción que excede la longitud máxima
- **WHEN** el cliente envía una solicitud `POST /api/tasks` con una descripción que supera los 2.000 caracteres
- **THEN** el sistema rechaza la solicitud con código HTTP 400 Bad Request indicando que la descripción excede el límite permitido y no persiste ninguna tarea

#### Scenario: Rechazo por fecha de vencimiento anterior a la actual
- **WHEN** el cliente envía una solicitud `POST /api/tasks` con una fecha de vencimiento que precede a la fecha actual
- **THEN** el sistema rechaza la solicitud con código HTTP 400 Bad Request indicando que la fecha de vencimiento debe ser igual o posterior a la actual y no persiste ninguna tarea

#### Scenario: Rechazo por valor de prioridad no soportado
- **WHEN** el cliente envía una solicitud con una prioridad distinta de "Low", "Medium" o "High" (por ejemplo, "Urgent")
- **THEN** el sistema rechaza la solicitud con código HTTP 400 Bad Request indicando que la prioridad no es válida y no persiste ninguna tarea

### Requirement: Persistencia durable y atomicidad de la tarea
El sistema DEBERÁ (SHALL) persistir la tarea en la base de datos de manera atómica cuando la creación sea válida. Las solicitudes inválidas o que fallen antes del commit no dejarán persistencia parcial. Ante fallos de comunicación durante el commit, el resultado puede ser incierto y reintentos del cliente podrían causar duplicados (idempotencia fuera de alcance).

#### Scenario: Confirmación de persistencia tras creación exitosa
- **WHEN** una solicitud de creación de tarea es validada y procesada exitosamente
- **THEN** la tarea queda almacenada de forma persistente con su identificador único, título, descripción, fecha de vencimiento, prioridad y estado inicial `Pending`

#### Scenario: Ausencia de persistencia ante solicitud fallida o inválida
- **WHEN** una solicitud de creación es rechazada por validación o experimenta un error antes de la confirmación en la base de datos
- **THEN** el sistema garantiza que no se persiste ningún registro ni persistencia parcial de la tarea en la base de datos

### Requirement: Manejo seguro de errores y restricción a interfaz local loopback
El sistema DEBERÁ (SHALL) responder con mensajes genéricos seguros ante fallos inesperados sin exponer detalles internos. En desarrollo, el sistema DEBERÁ (SHALL) fallar y abortar su inicio antes de aceptar solicitudes si la configuración efectiva de Kestrel permite escuchar fuera de interfaces locales de loopback (127.0.0.1 o ::1).

#### Scenario: Manejo seguro de fallos inesperados
- **WHEN** ocurre un error inesperado en el servidor durante el procesamiento de la creación de una tarea
- **THEN** el sistema responde con código HTTP 500 Internal Server Error con un mensaje genérico seguro, sin exponer trazas de pila (*stack traces*), cadenas de conexión ni detalles técnicos internos

#### Scenario: Aborto del inicio del servicio ante enlaces fuera de loopback
- **WHEN** la aplicación se inicia en el entorno de desarrollo y la configuración efectiva de escucha de Kestrel incluye interfaces no locales o públicas
- **THEN** el sistema aborta el inicio inmediatamente con un error explícito antes de aceptar solicitudes HTTP
