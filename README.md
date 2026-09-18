# InmobiliariaCC2

## Sistema Web para Gestión de Alquileres Temporales

Proyecto final desarrollado en **ASP.NET Core MVC** para la gestión integral de una inmobiliaria dedicada a alquileres temporales.

El sistema permite administrar propietarios, inquilinos, tipos de inmueble, inmuebles, solicitudes de reserva, reservas, pagos y usuarios. También incorpora autenticación por roles, control de disponibilidad, búsquedas, paginación e informes de gestión.

---

## Integrantes

- Guillermo Concha
- Natalia Camargo

---

## Tecnologías utilizadas

- ASP.NET Core MVC
- C#
- .NET
- MySQL
- MySQL Workbench
- MySql.Data / MySqlConnector
- HTML
- CSS
- Bootstrap
- Razor Views
- Git
- GitHub
- Visual Studio Code

El acceso a datos se realiza mediante SQL manual utilizando repositorios y objetos como:

- `MySqlConnection`
- `MySqlCommand`
- `MySqlDataReader`

No se utiliza Entity Framework para la persistencia de datos.

---

# Arquitectura del proyecto

El proyecto sigue una arquitectura basada en el patrón MVC:

## Models

Representan las entidades y modelos utilizados por el sistema.

Entre los principales modelos se encuentran:

- Propietario
- Inquilino
- TipoInmueble
- Inmueble
- SolicitudReserva
- Reserva
- Pago
- Usuario
- modelos auxiliares para informes

## Views

Contienen las interfaces desarrolladas mediante Razor, HTML y Bootstrap.

## Controllers

Gestionan las solicitudes HTTP, validaciones, autorización y comunicación entre las vistas y los repositorios.

## Repositories

Contienen el acceso a MySQL mediante consultas SQL manuales.

---

# Base de datos

La base de datos utilizada por el proyecto se denomina:

```text
inmobiliaria_cc2
```

El repositorio incluye el archivo:

```text
Inmobiliaria_CC2_Final.sql
```

Este archivo contiene la estructura y los datos necesarios para crear e inicializar la base utilizada por la aplicación.

---

# Tablas principales

La base de datos contiene las siguientes tablas:

1. `propietario`
2. `inquilino`
3. `tipoinmueble`
4. `inmueble`
5. `solicitudreserva`
6. `reserva`
7. `pago`
8. `usuario`

---

# Modelo de datos

## Propietario

Representa a los propietarios de los inmuebles administrados por la inmobiliaria.

Contiene información personal y de contacto y se relaciona con los inmuebles registrados.

## Inquilino

Representa a las personas que realizan reservas de los inmuebles.

Contiene datos personales, de contacto y domicilio de origen.

## TipoInmueble

Permite clasificar los inmuebles según su tipo.

Por ejemplo:

- Casa
- Departamento
- Cabaña
- Local
- Otros

## Inmueble

Representa las propiedades disponibles para alquiler temporal.

Entre sus datos se encuentran:

- Dirección
- Tipo de inmueble
- Propietario
- Cupo
- Coordenadas
- Precio por día
- Porcentaje mínimo de reserva
- Estado
- Fotografía

## SolicitudReserva

Permite registrar solicitudes relacionadas con la intención de reservar un inmueble.

## Reserva

Representa el alquiler temporal de un inmueble por parte de un inquilino.

Incluye:

- Inquilino
- Inmueble
- Fecha desde
- Fecha hasta
- Monto por día
- Estado
- Datos relacionados con creación y finalización
- Información necesaria para el seguimiento de la reserva

## Pago

Registra los pagos asociados a las reservas.

Permite mantener información sobre los importes abonados y su relación con la reserva correspondiente.

## Usuario

Representa a los usuarios que pueden autenticarse en el sistema.

Permite gestionar:

- Nombre
- Email
- Contraseña almacenada mediante hash
- Rol
- Estado
- Avatar

---

# Diagrama Entidad-Relación

El siguiente esquema representa de manera simplificada las principales relaciones de la base de datos:

```text
┌─────────────────┐
│   PROPIETARIO   │
└────────┬────────┘
         │ 1
         │
         │ N
┌────────▼────────┐       ┌──────────────────┐
│    INMUEBLE     │ N ──1 │  TIPO_INMUEBLE  │
└────────┬────────┘       └──────────────────┘
         │
         │ 1
         │
         │ N
┌────────▼────────┐
│     RESERVA     │
└───┬─────────┬───┘
    │         │
    │ N       │ 1
    │         │
    │ 1       │ N
┌───▼───────┐ ┌───────────▼──────┐
│ INQUILINO │ │       PAGO       │
└───────────┘ └──────────────────┘


INQUILINO ───── SOLICITUD_RESERVA ───── INMUEBLE


USUARIO
   │
   ├──── autenticación y roles
   │
   └──── trazabilidad de operaciones
```

Las claves primarias, claves foráneas y relaciones definitivas se encuentran especificadas en:

```text
Inmobiliaria_CC2_Final.sql
```

---

# Funcionalidades principales

## Gestión de Propietarios

El sistema permite:

- Registrar propietarios.
- Consultar propietarios.
- Modificar sus datos.
- Realizar baja lógica.
- Buscar propietarios.
- Paginar los resultados.
- Consultar los inmuebles asociados a un propietario.

---

## Gestión de Inquilinos

Permite:

- Registrar inquilinos.
- Consultar inquilinos.
- Modificar sus datos.
- Realizar baja lógica.
- Buscar por DNI, nombre, apellido, email, teléfono o dirección.
- Paginar los resultados.

---

## Gestión de Tipos de Inmueble

Permite administrar las diferentes categorías utilizadas para clasificar los inmuebles.

---

## Gestión de Inmuebles

Permite:

- Registrar inmuebles.
- Asociar un inmueble a un propietario.
- Asociar un tipo de inmueble.
- Definir cupo.
- Definir precio por día.
- Definir porcentaje mínimo de reserva.
- Registrar coordenadas.
- Cargar fotografías.
- Consultar detalles.
- Modificar información.
- Realizar baja lógica.
- Buscar inmuebles.
- Paginar resultados.

El sistema permite además buscar inmuebles disponibles para un período determinado.

---

# Disponibilidad

El sistema permite consultar inmuebles disponibles entre dos fechas.

La búsqueda utiliza las reservas existentes para evitar ofrecer inmuebles que se encuentren ocupados durante el período solicitado.

Esto permite controlar superposiciones de fechas y mejorar la gestión de disponibilidad.

---

# Solicitudes de reserva

El sistema incorpora la gestión de solicitudes de reserva como parte del proceso de alquiler temporal.

Las solicitudes permiten registrar la intención de reservar un inmueble y mantener el seguimiento correspondiente dentro del sistema.

---

# Gestión de Reservas

El módulo de reservas permite:

- Seleccionar un inquilino.
- Seleccionar un inmueble.
- Definir fecha de inicio.
- Definir fecha de finalización.
- Consultar disponibilidad.
- Evitar reservas incompatibles por superposición de fechas.
- Registrar el valor correspondiente al alquiler.
- Consultar reservas.
- Mantener el estado de cada reserva.

El valor del alquiler se relaciona con el precio por día definido para el inmueble y la duración de la estadía.

El sistema permite conocer el monto correspondiente a la reserva y el importe requerido según el porcentaje de reserva configurado para el inmueble.

---

# Pagos

El sistema cuenta con un módulo para registrar pagos asociados a las reservas.

Esto permite realizar el seguimiento económico de cada alquiler y conservar la relación entre el pago y la reserva correspondiente.

La información de pagos se utiliza además para la consulta y gestión administrativa.

---

# Usuarios y autenticación

El sistema utiliza autenticación mediante cookies.

Los usuarios poseen roles que permiten controlar el acceso a diferentes funcionalidades.

Los roles principales son:

## Administrador

Posee los permisos administrativos del sistema.

Puede acceder a las funciones de administración y mantenimiento habilitadas para su rol.

## Empleado

Puede acceder a las funciones operativas habilitadas para la gestión inmobiliaria.

## Visitante

Corresponde a un usuario no autenticado.

Puede acceder únicamente a las funciones públicas habilitadas por la aplicación.

---

# Perfil de usuario

Los usuarios autenticados disponen de un perfil personal.

El sistema permite:

- Consultar los datos del perfil.
- Modificar nombre.
- Modificar email.
- Cambiar contraseña.
- Cargar o modificar avatar.
- Visualizar el rol del usuario.

El rol no puede modificarse desde el perfil personal.

---

# Seguridad

Las contraseñas no se almacenan en texto plano.

El sistema utiliza hashing de contraseñas y validación de credenciales durante el inicio de sesión.

Las funcionalidades restringidas utilizan autorización mediante roles.

Ejemplo:

```csharp
[Authorize(Roles = "Administrador")]
```

o:

```csharp
[Authorize(Roles = "Administrador,Empleado")]
```

También se utiliza protección antifalsificación en formularios POST mediante:

```csharp
[ValidateAntiForgeryToken]
```

---

# Búsquedas y paginación

Los listados principales incorporan mecanismos de búsqueda.

En los módulos implementados con paginación, el procesamiento se realiza del lado del servidor.

Las consultas utilizan SQL con filtros y paginación mediante:

```sql
LIMIT
OFFSET
```

De esta manera no es necesario cargar todos los registros de la base de datos para mostrar una página de resultados.

Se implementaron búsquedas y paginación, entre otros, en los listados de:

- Inmuebles
- Propietarios
- Inquilinos

---

# Informes

El sistema incorpora un módulo específico de informes para facilitar la gestión de la inmobiliaria.

Entre los informes implementados se encuentran:

## Inmuebles más reservados

Presenta un ranking de inmuebles según la cantidad de reservas realizadas durante un período determinado.

## Inmuebles sin reservas

Permite consultar propiedades que no registraron reservas durante una determinada cantidad de días.

## Reservas vigentes

Muestra las reservas activas cuyo período de alquiler comprende la fecha actual.

## Próximos vencimientos

Permite consultar reservas cuya fecha de finalización se encuentra próxima.

La cantidad de días puede utilizarse como criterio para la consulta.

## Inmuebles y estado

Permite consultar los inmuebles registrados y filtrarlos según su estado.

## Inmuebles por propietario

Permite seleccionar un propietario y consultar los inmuebles asociados al mismo.

## Disponibilidad entre fechas

Permite determinar qué inmuebles se encuentran disponibles para un período determinado.

---

# Credenciales de prueba

Para facilitar la evaluación del sistema se incluyen usuarios de prueba.

## Administrador

```text
Email: admin@inmobiliaria.com
Contraseña: Admin123!
```

## Empleado

```text
Email: empleado@inmobiliaria.com
Contraseña: 123456
```

Estas credenciales están destinadas exclusivamente a la ejecución y evaluación académica del proyecto.

---

# Requisitos para ejecutar el proyecto

Para ejecutar el sistema se necesita:

- .NET SDK compatible con el proyecto.
- MySQL Server.
- MySQL Workbench o herramienta equivalente.
- Git, en caso de clonar el repositorio.
- Visual Studio Code, Visual Studio o IDE compatible con .NET.

---

# Instalación de la base de datos

El repositorio incluye:

```text
Inmobiliaria_CC2_Final.sql
```

Este archivo permite crear e inicializar la base de datos.

## Opción 1 - MySQL Workbench

1. Abrir MySQL Workbench.
2. Conectarse al servidor MySQL local.
3. Seleccionar:

```text
File → Open SQL Script
```

4. Abrir:

```text
Inmobiliaria_CC2_Final.sql
```

5. Ejecutar el script completo.

El script crea e inicializa la base:

```text
inmobiliaria_cc2
```

6. Actualizar el panel `Schemas`.
7. Verificar que aparezca la base `inmobiliaria_cc2` y sus tablas.

---

# Configuración de la conexión

La aplicación utiliza una cadena de conexión denominada:

```text
CadenaSQL
```

Por seguridad, la contraseña local de MySQL no debe almacenarse en el repositorio público.

Se recomienda utilizar **User Secrets**.

Desde la carpeta raíz del proyecto ejecutar:

```bash
dotnet user-secrets init
```

Luego configurar la cadena de conexión:

```bash
dotnet user-secrets set "ConnectionStrings:CadenaSQL" "Server=localhost;Port=3306;Database=inmobiliaria_cc2;User=root;Password=SU_PASSWORD;"
```

Reemplazar:

```text
SU_PASSWORD
```

por la contraseña correspondiente a la instalación local de MySQL.

Para verificar la configuración:

```bash
dotnet user-secrets list
```

---

# Restaurar dependencias

Desde la carpeta raíz del proyecto ejecutar:

```bash
dotnet restore
```

---

# Compilar el proyecto

Para comprobar que el proyecto compile correctamente:

```bash
dotnet build
```

---

# Ejecutar la aplicación

Ejecutar:

```bash
dotnet run
```

También puede utilizarse:

```bash
dotnet watch run
```

Luego abrir en el navegador la dirección indicada por ASP.NET Core en la consola.

---

# Estructura general

```text
InmobiliariaCC2/
│
├── Controllers/
│   ├── HomeController.cs
│   ├── PropietarioController.cs
│   ├── InquilinoController.cs
│   ├── TipoInmuebleController.cs
│   ├── InmuebleController.cs
│   ├── ReservaController.cs
│   ├── UsuarioController.cs
│   └── InformeController.cs
│
├── Models/
│   ├── Propietario.cs
│   ├── Inquilino.cs
│   ├── TipoInmueble.cs
│   ├── Inmueble.cs
│   ├── Reserva.cs
│   ├── Usuario.cs
│   └── modelos auxiliares
│
├── Repositories/
│   ├── RepositorioPropietario.cs
│   ├── RepositorioInquilino.cs
│   ├── RepositorioTipoInmueble.cs
│   ├── RepositorioInmueble.cs
│   ├── RepositorioReserva.cs
│   ├── RepositorioUsuario.cs
│   └── RepositorioInforme.cs
│
├── Views/
│   ├── Home/
│   ├── Propietario/
│   ├── Inquilino/
│   ├── TipoInmueble/
│   ├── Inmueble/
│   ├── Reserva/
│   ├── Usuario/
│   ├── Informe/
│   └── Shared/
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   ├── lib/
│   └── uploads/
│
├── .gitignore
├── Inmobiliaria_CC2_Final.sql
├── InmobiliariaCC2.csproj
├── Program.cs
├── appsettings.json
└── README.md
```

---

# Archivo .gitignore

El proyecto incluye un archivo `.gitignore` adaptado al entorno .NET y Visual Studio.

Entre otros elementos, evita versionar:

- `bin/`
- `obj/`
- `.vs/`
- archivos temporales
- resultados de compilación
- configuraciones locales
- archivos `.env`
- paquetes y cachés generados

---

# Consideraciones finales

El proyecto integra los principales procesos necesarios para la administración de alquileres temporales mediante una aplicación web desarrollada con ASP.NET Core MVC.

La solución contempla la gestión de propietarios, inquilinos, inmuebles, solicitudes, reservas, pagos y usuarios, junto con mecanismos de autenticación, autorización por roles, control de disponibilidad, búsqueda, paginación e informes.

La persistencia se implementa mediante MySQL y consultas SQL manuales organizadas en repositorios, manteniendo separadas las responsabilidades de acceso a datos, lógica de control y presentación.

El archivo `Inmobiliaria_CC2_Final.sql` permite reconstruir la base utilizada para la evaluación, mientras que las credenciales incluidas en este documento permiten acceder a los perfiles de Administrador y Empleado.