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

Las relaciones mediante claves foráneas son:

```text
Inmueble.IdPropietario
        ↓
Propietario.IdPropietario


Inmueble.IdTipo
        ↓
TipoInmueble.IdTipo


Reserva.IdInquilino
        ↓
Inquilino.IdInquilino


Reserva.IdInmueble
        ↓
Inmueble.IdInmueble
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

De esta manera se evita que un mismo inmueble tenga dos reservas activas para períodos que se superponen.

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

Para realizar el cálculo, el sistema determina:

```text
Días pactados
      ↓
Días cumplidos
      ↓
Días restantes
      ↓
Monto de los días restantes
      ↓
Aplicación del 50 % o 25 %
      ↓
Multa final
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

---

# Puesta en marcha del proyecto

Para ejecutar el proyecto por primera vez en una computadora nueva se deben realizar los siguientes pasos.

## 1. Requisitos previos

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

El proyecto fue desarrollado utilizando .NET 10.

## 2. Clonar el repositorio

Desde una terminal ejecutar:

```powershell
git clone https://github.com/LaloSL/InmobiliariaCC2.git
```

Luego ingresar a la carpeta del proyecto:

```powershell
cd InmobiliariaCC2
```

También se puede clonar el repositorio utilizando GitHub Desktop.

---

# Creación de la base de datos

Dentro de los archivos del repositorio se encuentra incluido un archivo `.sql` con la estructura de la base de datos.

Este archivo puede abrirse y ejecutarse directamente utilizando MySQL Workbench.

También se incluye a continuación el query completo necesario para crear la base de datos desde cero.

## Script completo de creación

Abrir MySQL Workbench, crear una nueva pestaña SQL, copiar el siguiente script y ejecutarlo.

```sql
CREATE DATABASE IF NOT EXISTS inmobiliaria_cc2;

USE inmobiliaria_cc2;


-- =====================================================
-- TABLA: TipoInmueble
-- =====================================================

CREATE TABLE TipoInmueble
(
    IdTipo INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Estado BOOLEAN NOT NULL DEFAULT TRUE
);


-- =====================================================
-- TABLA: Propietario
-- =====================================================

CREATE TABLE Propietario
(
    IdPropietario INT AUTO_INCREMENT PRIMARY KEY,
    Dni VARCHAR(20) NOT NULL,
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Telefono VARCHAR(30) NOT NULL,
    Direccion VARCHAR(150) NULL,
    Estado BOOLEAN NOT NULL DEFAULT TRUE,
    FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);


-- =====================================================
-- TABLA: Inquilino
-- =====================================================

CREATE TABLE Inquilino
(
    IdInquilino INT AUTO_INCREMENT PRIMARY KEY,
    Dni VARCHAR(20) NOT NULL,
    Nombre VARCHAR(50) NOT NULL,
    Apellido VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Telefono VARCHAR(30) NOT NULL,
    DireccionOrigen VARCHAR(150) NULL,
    Estado BOOLEAN NOT NULL DEFAULT TRUE,
    FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);


-- =====================================================
-- TABLA: Inmueble
-- =====================================================

CREATE TABLE Inmueble
(
    IdInmueble INT AUTO_INCREMENT PRIMARY KEY,
    Direccion VARCHAR(255) NOT NULL,
    Cupo INT NOT NULL,
    IdTipo INT NOT NULL,
    Coordenadas VARCHAR(100) NULL,
    PrecioDia DECIMAL(10,2) NOT NULL,
    IdPropietario INT NOT NULL,
    Estado BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT FK_Inmueble_TipoInmueble
        FOREIGN KEY (IdTipo)
        REFERENCES TipoInmueble(IdTipo),

    CONSTRAINT FK_Inmueble_Propietario
        FOREIGN KEY (IdPropietario)
        REFERENCES Propietario(IdPropietario)
);


-- =====================================================
-- TABLA: Reserva
-- =====================================================

CREATE TABLE Reserva
(
    IdReserva INT AUTO_INCREMENT PRIMARY KEY,
    IdInquilino INT NOT NULL,
    IdInmueble INT NOT NULL,
    MontoDia DECIMAL(10,2) NOT NULL,
    FechaDesde DATE NOT NULL,
    FechaHasta DATE NOT NULL,
    FechaTerminacionAnticipada DATE NULL,
    Multa DECIMAL(10,2) NOT NULL DEFAULT 0,
    Estado BOOLEAN NOT NULL DEFAULT TRUE,

    CONSTRAINT FK_Reserva_Inquilino
        FOREIGN KEY (IdInquilino)
        REFERENCES Inquilino(IdInquilino),

    CONSTRAINT FK_Reserva_Inmueble
        FOREIGN KEY (IdInmueble)
        REFERENCES Inmueble(IdInmueble)
);
```

## Orden de creación

Las tablas que son referenciadas mediante claves foráneas deben existir antes que las tablas que dependen de ellas.

La estructura general es:

```text
TipoInmueble ──────┐
                   │
                   ▼
                Inmueble
                   ▲
                   │
Propietario ───────┘
                   │
                   ▼
                Reserva
                   ▲
                   │
Inquilino ─────────┘
```

Por este motivo:

- `TipoInmueble` debe existir antes de crear `Inmueble`.
- `Propietario` debe existir antes de crear `Inmueble`.
- `Inquilino` debe existir antes de crear `Reserva`.
- `Inmueble` debe existir antes de crear `Reserva`.

## Procedimiento utilizando MySQL Workbench

1. Abrir MySQL Workbench.
2. Conectarse al servidor local de MySQL.
3. Abrir una nueva pestaña SQL.
4. Abrir el archivo `.sql` incluido en el repositorio o copiar el script anterior.
5. Ejecutar el script completo.
6. Actualizar la lista de bases de datos.
7. Verificar que exista `inmobiliaria_cc2`.

La estructura esperada será:

```text
inmobiliaria_cc2
│
├── TipoInmueble
├── Propietario
├── Inquilino
├── Inmueble
└── Reserva
```

Para verificar las tablas desde MySQL también se puede ejecutar:

```sql
USE inmobiliaria_cc2;

SHOW TABLES;
```

---

# Configuración de la conexión a MySQL

Por razones de seguridad, la contraseña de MySQL no se almacena directamente dentro de `appsettings.json` y tampoco se sube al repositorio de GitHub.

El proyecto utiliza User Secrets de .NET para almacenar localmente la cadena de conexión.

Cada integrante debe configurar en su computadora su propia contraseña de MySQL.

Desde una terminal ubicada dentro de la carpeta del proyecto ejecutar:

```powershell
dotnet user-secrets set "ConnectionStrings:CadenaSQL" "Server=localhost;Port=3306;Database=inmobiliaria_cc2;User=root;Password=TU_CLAVE;"
```

Se debe reemplazar:

```text
TU_CLAVE
```

por la contraseña correspondiente al usuario local de MySQL.

La estructura de la cadena de conexión es:

```text
Server=localhost
Port=3306
Database=inmobiliaria_cc2
User=root
Password=contraseña local
```

La aplicación obtiene posteriormente la cadena mediante:

```csharp
configuration.GetConnectionString("CadenaSQL")
```

La configuración de User Secrets queda almacenada de manera local en cada computadora.

La contraseña no se incorpora al repositorio Git y no debe escribirse directamente dentro del código fuente.

## Verificar User Secrets

Para comprobar que la cadena de conexión fue guardada correctamente:

```powershell
dotnet user-secrets list
```

Deberá aparecer una entrada similar a:

```text
ConnectionStrings:CadenaSQL
```

Importante: no publicar ni compartir la contraseña de MySQL mediante GitHub, capturas de pantalla o archivos del proyecto.

---

# Restaurar dependencias

Una vez clonado el proyecto y creada la base de datos, desde la carpeta principal ejecutar:

```powershell
dotnet restore
```

Este comando restaura los paquetes necesarios para compilar y ejecutar la aplicación.

El paquete utilizado para realizar la conexión con MySQL es:

```text
MySql.Data
```

Para verificar los paquetes instalados:

```powershell
dotnet list package
```

---

# Compilar el proyecto

Antes de ejecutar la aplicación se recomienda comprobar que el proyecto compile correctamente.

Ejecutar:

```powershell
dotnet build
```

Si no existen problemas, la compilación deberá finalizar sin errores.

Este paso permite detectar errores de código antes de iniciar la aplicación.

---

# Ejecutar el proyecto

Para iniciar la aplicación ejecutar:

```powershell
dotnet run
```

Una vez iniciada, la terminal mostrará una dirección local similar a:

```text
http://localhost:5225
```

El puerto puede variar dependiendo de la configuración de cada computadora.

La dirección mostrada en la terminal debe abrirse desde un navegador web.

Para detener la aplicación utilizar:

```text
Ctrl + C
```

---

# Resumen para levantar el proyecto desde cero

El procedimiento general para ejecutar el sistema en una computadora nueva es:

```text
Clonar repositorio
        ↓
Crear la base de datos
        ↓
Ejecutar el script SQL
        ↓
Configurar User Secrets
        ↓
dotnet restore
        ↓
dotnet build
        ↓
dotnet run
        ↓
Abrir la aplicación en el navegador
```

Los comandos principales son:

```powershell
dotnet restore
dotnet build
dotnet run
```

Para configurar la conexión:

```powershell
dotnet user-secrets set "ConnectionStrings:CadenaSQL" "Server=localhost;Port=3306;Database=inmobiliaria_cc2;User=root;Password=TU_CLAVE;"
```

Para verificar User Secrets:

```powershell
dotnet user-secrets list
```

Para verificar los paquetes:

```powershell
dotnet list package
```

---

# Organización del proyecto

La estructura principal del proyecto es:

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

---

# Arquitectura utilizada

El proyecto utiliza el patrón MVC.

```text
             Usuario
                │
                ▼
            Controller
             /       \
            ▼         ▼
         Model       View
            │
            ▼
       Repository
            │
            ▼
          MySQL
```

## Models

Representan las entidades principales del sistema:

- Propietario.
- Inquilino.
- TipoInmueble.
- Inmueble.
- Reserva.

Los modelos contienen las propiedades que representan los datos utilizados por la aplicación.

## Controllers

Los controladores reciben las solicitudes realizadas por el usuario desde las vistas y coordinan las diferentes operaciones del sistema.

Por ejemplo:

```text
Usuario
   ↓
ReservaController
   ↓
RepositorioReserva
   ↓
MySQL
```

## Repositories

Los repositorios contienen las operaciones necesarias para acceder a la base de datos.

En ellos se utilizan:

```text
MySqlConnection
MySqlCommand
MySqlDataReader
```

y se escriben manualmente consultas SQL para:

- Listar registros.
- Buscar registros por ID.
- Insertar registros.
- Modificar registros.
- Realizar bajas lógicas.
- Realizar INNER JOIN.
- Verificar disponibilidad.
- Registrar reservas.
- Finalizar reservas anticipadamente.

Ejemplo general:

```csharp
using (var connection = new MySqlConnection(_connectionString))
{
    var sql = @"SELECT *
                FROM Propietario
                WHERE Estado = 1;";

    using (var command = new MySqlCommand(sql, connection))
    {
        connection.Open();

        using (var reader = command.ExecuteReader())
        {
            // Lectura de los registros
        }
    }
}
```

## Views

Las vistas contienen las interfaces utilizadas por el usuario.

Entre otras operaciones permiten:

- Visualizar listados.
- Crear registros.
- Editar registros.
- Consultar detalles.
- Seleccionar propietarios.
- Seleccionar inquilinos.
- Seleccionar tipos de inmueble.
- Seleccionar inmuebles.
- Registrar reservas.
- Consultar detalles de reservas.

Las vistas utilizan Razor, HTML, Bootstrap y JavaScript.

---

# Baja lógica

En diferentes entidades del sistema no se realiza una eliminación física del registro.

En cambio, se modifica el campo:

```text
Estado
```

Por ejemplo:

```sql
UPDATE Propietario
SET Estado = 0
WHERE IdPropietario = @id;
```

De esta manera el registro permanece almacenado en la base de datos, pero deja de aparecer entre los registros activos.

---

# Uso de JavaScript

JavaScript es utilizado en determinadas vistas para realizar operaciones dinámicas.

Por ejemplo, al crear una reserva:

```text
Seleccionar Tipo de Inmueble
             ↓
JavaScript realiza una solicitud
             ↓
ReservaController
             ↓
ObtenerInmueblesPorTipo()
             ↓
RepositorioInmueble
             ↓
MySQL
             ↓
Se cargan los inmuebles correspondientes
```

De esta manera, al seleccionar un tipo de inmueble, el usuario puede visualizar únicamente los inmuebles pertenecientes a ese tipo.

Además se muestra información como:

```text
Dirección | Capacidad | Precio por día
```

---

# Control de versiones

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

Para subir los cambios:

```powershell
git push origin main
```

El flujo habitual de trabajo es:

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

No se recomienda utilizar `force push` para resolver conflictos en un repositorio compartido.

---

# Importante para nuevos integrantes

Antes de comenzar a trabajar con el proyecto se debe comprobar:

1. Tener instalado .NET 10.
2. Tener MySQL instalado y funcionando.
3. Tener MySQL Workbench o una herramienta equivalente.
4. Tener Git instalado.
5. Clonar el repositorio.
6. Ejecutar el archivo SQL incluido en el proyecto o utilizar el script disponible en este README.
7. Verificar que exista la base `inmobiliaria_cc2`.
8. Verificar que estén creadas todas las tablas.
9. Configurar `ConnectionStrings:CadenaSQL` mediante User Secrets.
10. Ejecutar `dotnet restore`.
11. Ejecutar `dotnet build`.
12. Ejecutar `dotnet run`.

La contraseña de MySQL nunca debe escribirse directamente dentro del código fuente ni subirse al repositorio de GitHub.

---

# Comandos útiles

Comprobar la versión de .NET:

```powershell
dotnet --version
```

Restaurar paquetes:

```powershell
dotnet restore
```

Compilar:

```powershell
dotnet build
```

Ejecutar:

```powershell
dotnet run
```

Ver paquetes instalados:

```powershell
dotnet list package
```

Ver User Secrets:

```powershell
dotnet user-secrets list
```

Ver estado de Git:

```powershell
git status
```

Actualizar el repositorio:

```powershell
git pull origin main
```

Agregar cambios:

```powershell
git add .
```

Crear un commit:

```powershell
git commit -m "Descripcion del cambio"
```

Subir cambios:

```powershell
git push origin main
```

---

# Objetivo académico

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