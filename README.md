# InmobiliariaCC2

Sistema web desarrollado en ASP.NET Core MVC para la gestión de alquileres temporarios.

El proyecto permite administrar propietarios, inquilinos, tipos de inmueble, inmuebles y reservas, utilizando una base de datos MySQL y consultas SQL escritas manualmente.

## Integrantes

- Guillermo Concha
- Natalia Camargo

## Tecnologías utilizadas

- .NET 10
- ASP.NET Core MVC
- C#
- MySQL
- MySql.Data
- Bootstrap
- HTML
- CSS
- JavaScript
- Git
- GitHub

## Acceso a datos

El proyecto no utiliza Entity Framework Core para realizar las operaciones sobre la base de datos.

El acceso a MySQL se realiza mediante:

- MySqlConnection
- MySqlCommand
- MySqlDataReader
- ExecuteReader()
- ExecuteScalar()
- ExecuteNonQuery()

Las consultas SQL, tales como SELECT, INSERT, UPDATE e INNER JOIN, se encuentran escritas manualmente dentro de los repositorios.

La estructura general de acceso a datos es:

```text
Controller
    ↓
Repository
    ↓
MySqlConnection / MySqlCommand
    ↓
SQL
    ↓
MySQL
```

## Base de datos

La base de datos utilizada por el proyecto se denomina:

```text
inmobiliaria_cc2
```

Actualmente contiene las siguientes tablas principales:

```text
Propietario
Inquilino
TipoInmueble
Inmueble
Reserva
```

### Relaciones principales

```text
Propietario
     │
     │ IdPropietario
     ▼
  Inmueble ◄──────── TipoInmueble
     │
     │ IdInmueble
     ▼
   Reserva
     ▲
     │ IdInquilino
     │
 Inquilino
```

## Funcionalidades del sistema

### Propietarios

El módulo de propietarios permite:

- Listar propietarios.
- Registrar nuevos propietarios.
- Consultar detalles.
- Modificar datos.
- Realizar baja lógica.

La baja lógica modifica el campo `Estado` en lugar de eliminar físicamente el registro de la base de datos.

### Inquilinos

El módulo de inquilinos permite:

- Listar inquilinos.
- Registrar nuevos inquilinos.
- Consultar detalles.
- Modificar información.
- Realizar baja lógica.

### Tipos de inmueble

Permite administrar los distintos tipos de inmueble disponibles.

Por ejemplo:

- Casa.
- Departamento.
- Cabaña.

Los tipos de inmueble también utilizan baja lógica mediante el campo `Estado`.

### Inmuebles

Cada inmueble se encuentra relacionado con:

- Un propietario.
- Un tipo de inmueble.

Entre sus datos principales se encuentran:

- Dirección.
- Cupo de personas.
- Tipo de inmueble.
- Coordenadas.
- Precio por día.
- Propietario.
- Estado.

Al registrar un inmueble, el sistema permite seleccionar el propietario correspondiente mostrando su nombre y apellido.

### Reservas

Las reservas relacionan:

```text
Inquilino + Inmueble + Período
```

Para registrar una reserva se selecciona:

1. Inquilino.
2. Tipo de inmueble.
3. Inmueble.
4. Fecha desde.
5. Fecha hasta.

El sistema muestra el nombre completo del inquilino.

Al seleccionar un tipo de inmueble, se filtran automáticamente los inmuebles pertenecientes a esa categoría.

Para cada inmueble se puede visualizar información como:

- Dirección.
- Capacidad de personas.
- Precio por día.

El monto diario de la reserva no es ingresado manualmente por el usuario.

El sistema obtiene automáticamente el precio por día configurado en el inmueble seleccionado.

## Control de disponibilidad

Antes de registrar una nueva reserva, el sistema verifica si el inmueble seleccionado posee otra reserva activa que se superponga con las fechas ingresadas.

La validación utiliza la siguiente lógica:

```text
FechaDesde existente < FechaHasta nueva

Y

FechaHasta existente > FechaDesde nueva
```

Si existe una superposición de fechas, el sistema impide registrar la nueva reserva.

## Finalización anticipada de una reserva

Una reserva activa puede finalizarse antes de la fecha originalmente pactada.

El sistema permite registrar la fecha de finalización anticipada y calcula automáticamente una penalidad.

### Regla de negocio

Si el inquilino cumplió menos de la mitad del período reservado:

```text
Multa = 50 % del valor correspondiente a los días restantes
```

Si el inquilino cumplió la mitad o más del período reservado:

```text
Multa = 25 % del valor correspondiente a los días restantes
```

Al finalizar anticipadamente una reserva se almacenan:

- Fecha de terminación anticipada.
- Multa calculada.
- Estado de la reserva.

En el detalle de la reserva se puede visualizar:

- Fecha de inicio.
- Fecha pactada de finalización.
- Fecha de finalización anticipada.
- Días pactados.
- Días cumplidos.
- Días restantes.
- Porcentaje de penalidad aplicado.
- Monto de la multa.

## Puesta en marcha del proyecto

Para ejecutar el proyecto por primera vez en una computadora nueva se deben realizar los siguientes pasos.

### 1. Requisitos previos

Antes de comenzar se debe contar con:

- .NET 10.
- MySQL.
- MySQL Workbench o una herramienta equivalente.
- Git.
- Un navegador web.

Opcionalmente se puede utilizar:

- Visual Studio Code.
- GitHub Desktop.

Para comprobar la versión instalada de .NET:

```powershell
dotnet --version
```

### 2. Clonar el repositorio

Desde una terminal ejecutar:

```powershell
git clone https://github.com/LaloSL/InmobiliariaCC2.git
```

Luego ingresar a la carpeta del proyecto:

```powershell
cd InmobiliariaCC2
```

También se puede clonar el repositorio utilizando GitHub Desktop.

## Creación de la base de datos

Dentro de los archivos incluidos en el repositorio se encuentra el script SQL necesario para crear la base de datos utilizada por la aplicación.

Quien clone el proyecto debe localizar el archivo `.sql` incluido en el repositorio y ejecutarlo utilizando MySQL Workbench.

El script permite generar la base de datos:

```text
inmobiliaria_cc2
```

junto con las tablas necesarias para el funcionamiento del sistema.

Entre las tablas principales se encuentran:

```text
Propietario
Inquilino
TipoInmueble
Inmueble
Reserva
```

Por lo tanto, no es necesario crear manualmente cada una de las tablas.

### Procedimiento sugerido

1. Abrir MySQL Workbench.
2. Conectarse al servidor local de MySQL.
3. Abrir el archivo `.sql` incluido en el proyecto.
4. Ejecutar el script completo.
5. Actualizar la lista de bases de datos.
6. Verificar que aparezca `inmobiliaria_cc2`.

La estructura esperada será similar a:

```text
inmobiliaria_cc2
│
├── Propietario
├── Inquilino
├── TipoInmueble
├── Inmueble
└── Reserva
```

## Configuración de la conexión a MySQL

Por razones de seguridad, la contraseña de MySQL no se almacena directamente dentro de `appsettings.json` y tampoco se debe subir al repositorio de GitHub.

El proyecto utiliza User Secrets de .NET para almacenar localmente la cadena de conexión.

Cada integrante debe configurar en su propia computadora la contraseña correspondiente a su instalación de MySQL.

Desde una terminal ubicada dentro de la carpeta del proyecto ejecutar:

```powershell
dotnet user-secrets set "ConnectionStrings:CadenaSQL" "Server=localhost;Port=3306;Database=inmobiliaria_cc2;User=root;Password=TU_CLAVE;"
```

Se debe reemplazar:

```text
TU_CLAVE
```

por la contraseña local de MySQL.

Por ejemplo, la estructura de la cadena utilizada por el sistema es:

```text
Server=localhost
Port=3306
Database=inmobiliaria_cc2
User=root
Password=contraseña local
```

La aplicación recupera posteriormente la cadena de conexión mediante:

```csharp
configuration.GetConnectionString("CadenaSQL")
```

La contraseña configurada mediante User Secrets permanece almacenada de forma local y no se incorpora al repositorio Git.

### Verificar User Secrets

Para comprobar que la cadena de conexión fue guardada correctamente se puede ejecutar:

```powershell
dotnet user-secrets list
```

Debería aparecer:

```text
ConnectionStrings:CadenaSQL
```

junto con la cadena configurada.

Importante: no compartir capturas ni publicar la contraseña de MySQL en GitHub.

## Restaurar dependencias

Una vez clonada la aplicación y configurada la base de datos, desde la carpeta principal del proyecto ejecutar:

```powershell
dotnet restore
```

Este comando restaura los paquetes necesarios para compilar y ejecutar el proyecto.

El paquete utilizado para la conexión con MySQL es:

```text
MySql.Data
```

Para verificar los paquetes instalados:

```powershell
dotnet list package
```

## Compilar el proyecto

Antes de ejecutar la aplicación se recomienda realizar una compilación:

```powershell
dotnet build
```

Si todo está correctamente configurado, el comando deberá finalizar sin errores.

Por ejemplo:

```text
Compilación correcta.
0 errores
```

Este paso permite detectar posibles problemas de código antes de iniciar la aplicación.

## Ejecutar el proyecto

Para iniciar la aplicación ejecutar:

```powershell
dotnet run
```

Una vez iniciada, la terminal mostrará una dirección local similar a:

```text
http://localhost:5225
```

La dirección indicada debe abrirse desde el navegador.

El puerto puede variar dependiendo de la configuración de cada computadora.

Para detener la aplicación:

```text
Ctrl + C
```

## Resumen para ejecutar el proyecto

Luego de clonar el repositorio, el procedimiento general es:

```text
1. Ejecutar el archivo SQL incluido en el repositorio.
             ↓
2. Crear la base inmobiliaria_cc2.
             ↓
3. Configurar User Secrets.
             ↓
4. Restaurar dependencias.
             ↓
5. Compilar.
             ↓
6. Ejecutar.
```

Los comandos principales son:

```powershell
dotnet restore
dotnet build
dotnet run
```

Para configurar la conexión a MySQL:

```powershell
dotnet user-secrets set "ConnectionStrings:CadenaSQL" "Server=localhost;Port=3306;Database=inmobiliaria_cc2;User=root;Password=TU_CLAVE;"
```

Para verificar la configuración:

```powershell
dotnet user-secrets list
```

Para verificar los paquetes instalados:

```powershell
dotnet list package
```

## Organización del proyecto

La estructura principal es:

```text
InmobiliariaCC2
│
├── Controllers
│   ├── PropietarioController.cs
│   ├── InquilinoController.cs
│   ├── TipoInmuebleController.cs
│   ├── InmuebleController.cs
│   └── ReservaController.cs
│
├── Models
│   ├── Propietario.cs
│   ├── Inquilino.cs
│   ├── TipoInmueble.cs
│   ├── Inmueble.cs
│   └── Reserva.cs
│
├── Repositories
│   ├── RepositorioPropietario.cs
│   ├── RepositorioInquilino.cs
│   ├── RepositorioTipoInmueble.cs
│   ├── RepositorioInmueble.cs
│   └── RepositorioReserva.cs
│
├── Views
│   ├── Home
│   ├── Propietario
│   ├── Inquilino
│   ├── TipoInmueble
│   ├── Inmueble
│   ├── Reserva
│   └── Shared
│
├── wwwroot
├── Program.cs
├── appsettings.json
├── InmobiliariaCC2.csproj
├── README.md
└── archivo de base de datos .sql
```

## Arquitectura utilizada

El proyecto utiliza el patrón MVC:

```text
Model
  ↑
  │
Controller
  │
  ↓
View
```

Para el acceso a la base de datos se incorporan repositorios:

```text
Navegador
    ↓
Controller
    ↓
Repositorio
    ↓
MySqlCommand
    ↓
Consulta SQL
    ↓
MySQL
```

### Models

Representan las entidades principales del sistema:

- Propietario.
- Inquilino.
- TipoInmueble.
- Inmueble.
- Reserva.

### Controllers

Reciben las solicitudes realizadas desde las vistas y coordinan la lógica necesaria para responder al usuario.

### Repositories

Contienen las operaciones de acceso a MySQL.

En ellos se escriben manualmente las consultas SQL necesarias para:

- Consultar registros.
- Insertar registros.
- Modificar registros.
- Realizar bajas lógicas.
- Consultar relaciones entre tablas.
- Verificar disponibilidad de inmuebles.

### Views

Contienen las interfaces que utiliza el usuario para interactuar con el sistema.

## Control de versiones

El proyecto utiliza Git y GitHub para el control de versiones y el trabajo colaborativo.

Antes de comenzar a trabajar se recomienda actualizar la copia local:

```powershell
git pull origin main
```

Para verificar los archivos modificados:

```powershell
git status
```

Para agregar los cambios:

```powershell
git add .
```

Para realizar un commit:

```powershell
git commit -m "Descripcion del cambio"
```

Para subir los cambios al repositorio:

```powershell
git push origin main
```

Flujo habitual:

```text
git pull origin main
        ↓
Realizar modificaciones
        ↓
git status
        ↓
git add .
        ↓
git commit
        ↓
git push origin main
```

Si otro integrante realizó modificaciones en el repositorio remoto, los cambios deben integrarse antes de realizar un nuevo `push`.

No se recomienda utilizar `force push` para resolver conflictos en el repositorio compartido.

## Importante para nuevos integrantes

Antes de comenzar a trabajar con el proyecto se debe comprobar:

1. Tener instalado .NET 10.
2. Tener MySQL instalado y funcionando.
3. Tener acceso al repositorio.
4. Clonar el proyecto.
5. Ejecutar el archivo SQL incluido en el repositorio.
6. Verificar que exista la base `inmobiliaria_cc2`.
7. Configurar `ConnectionStrings:CadenaSQL` mediante User Secrets.
8. Ejecutar `dotnet restore`.
9. Ejecutar `dotnet build`.
10. Ejecutar `dotnet run`.

La contraseña de MySQL nunca debe escribirse directamente dentro del código fuente ni subirse a GitHub.

## Objetivo académico

El proyecto fue desarrollado como trabajo académico de programación web.

Su desarrollo permite aplicar conceptos relacionados con:

- ASP.NET Core MVC.
- Programación en C#.
- Bases de datos MySQL.
- Consultas SQL manuales.
- Patrón MVC.
- Repositorios.
- Relaciones entre tablas.
- Claves primarias y foráneas.
- Validaciones.
- Reglas de negocio.
- JavaScript.
- Git.
- GitHub.
- Trabajo colaborativo.

El objetivo principal es comprender cómo interactúan las distintas capas de una aplicación web y cómo se realiza el acceso a una base de datos mediante consultas SQL desarrolladas manualmente.