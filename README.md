# TP5 Programación Móvil - API

API REST para administrar usuarios, categorías, clientes, proveedores, productos, ingresos y ventas.

## Diagrama ER

![Diagrama de relaciones](Docs/DiagramaTPN5LopezCarola.png)

## Tecnologías

- .NET 10 y ASP.NET Core
- Entity Framework Core con PostgreSQL (Npgsql)
- Autenticación JWT
- Serilog para logs

## URLs

- Producción: `https://tp5programacion.runasp.net`
- Todos los endpoints se encuentran debajo de `/api`.

Por ejemplo: `https://tp5programacion.runasp.net/api/auth/login`.

## Ejecución local

La API requiere una cadena de conexión PostgreSQL y la configuración JWT. No se deben guardar secretos en `appsettings.json`; para desarrollo se pueden cargar con User Secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=...;Database=...;Username=...;Password=...;SSL Mode=Require" --project API
dotnet user-secrets set "JwtSettings:Key" "una-clave-secreta-larga" --project API
dotnet user-secrets set "JwtSettings:Issuer" "TP5Programacion" --project API
dotnet user-secrets set "JwtSettings:Audience" "TP5Programacion" --project API
dotnet user-secrets set "JwtSettings:ExpiryMinutes" "60" --project API
```

Para crear o actualizar el esquema:

```powershell
dotnet ef database update --project API --startup-project API
```

Para iniciar la API:

```powershell
dotnet run --project API
```

## Autenticación y permisos

`POST /api/auth/register` y `POST /api/auth/login` son públicos. Ambos devuelven un JWT cuando la operación resulta exitosa.

El resto de los controladores requiere autenticación. En las requests protegidas se debe enviar:

```http
Authorization: Bearer <token>
```

El token incluye el identificador del usuario y uno de estos roles:

| Rol | Alcance |
| --- | --- |
| `Visitante` | Puede consultar categorías, clientes, productos, ingresos y ventas, además de actualizar o darse de baja a sí mismo. |
| `Administrador` | Tiene todos los permisos de consulta y puede crear, actualizar, eliminar, dar de baja, registrar movimientos y administrar usuarios. |

Las operaciones que modifican inventario, clientes, categorías, productos, proveedores, ventas o las cuentas de otros usuarios requieren el rol `Administrador`.

## Respuestas y paginado

Los listados paginados devuelven una estructura con `numeroPagina`, `tamanoPagina`, `totalRegistros`, `totalPaginas`, `tienePaginaAnterior`, `tienePaginaPosterior` y `datos`.

Los valores comunes de paginado son:

| Parámetro | Comportamiento |
| --- | --- |
| `numeroPagina` | Por defecto `1`; valores menores a uno también se convierten en `1`. |
| `tamanoPagina` | Por defecto `20`; se limita a un máximo de `50`. |
| `direccion` | `asc` o `desc`; por defecto `desc` en los listados que ordenan por dirección. |

Los estados más frecuentes son `200 OK`, `201 Created`, `204 No Content`, `400 Bad Request`, `401 Unauthorized`, `403 Forbidden`, `404 Not Found`, `409 Conflict` y `500 Internal Server Error`.

## Endpoints

En las tablas, **Autenticado** significa cualquier usuario con JWT y **Admin** significa JWT con rol `Administrador`.

### Autorización

Requests de Bruno: [Auth](https://github.com/juliancarola04/BrunoCollectionTP5/tree/main/Auth).

| Método | Ruta | Acceso | Descripción |
| --- | --- | --- | --- |
| POST | `/api/auth/register` | Público | Registra un usuario y devuelve un token. |
| POST | `/api/auth/login` | Público | Inicia sesión y devuelve un token. |

### Categorías

Requests de Bruno: [Categorias](https://github.com/juliancarola04/BrunoCollectionTP5/tree/main/Categorias).

| Método | Ruta | Acceso | Descripción |
| --- | --- | --- | --- |
| GET | `/api/categorias` | Autenticado | Lista categorías paginadas. |
| GET | `/api/categorias/{id}` | Autenticado | Obtiene una categoría. |
| POST | `/api/categorias` | Admin | Crea una categoría. |
| PUT | `/api/categorias/{id}` | Admin | Actualiza una categoría. |
| DELETE | `/api/categorias/{id}` | Admin | Elimina una categoría sin productos asociados. |

Parámetros del listado: `buscar` por nombre; `ordenarPor` acepta `id` o `nombre`; `direccion` acepta `asc` o `desc`.

### Clientes

Requests de Bruno: [Clientes](https://github.com/juliancarola04/BrunoCollectionTP5/tree/main/Clientes).

| Método | Ruta | Acceso | Descripción |
| --- | --- | --- | --- |
| GET | `/api/clientes` | Autenticado | Lista clientes paginados. |
| GET | `/api/clientes/{id}` | Autenticado | Obtiene un cliente. |
| POST | `/api/clientes` | Admin | Crea un cliente. |
| PUT | `/api/clientes/{id}` | Admin | Actualiza un cliente. |
| DELETE | `/api/clientes/{id}` | Admin | Elimina un cliente sin ventas asociadas. |

Parámetros del listado: `buscar` por nombre, DNI o email; `ordenarPor` acepta `id`, `nombre` o `email`; `direccion` acepta `asc` o `desc`.

### Productos

Requests de Bruno: [Productos](https://github.com/juliancarola04/BrunoCollectionTP5/tree/main/Productos).

| Método | Ruta | Acceso | Descripción |
| --- | --- | --- | --- |
| GET | `/api/productos` | Autenticado | Lista productos paginados. |
| GET | `/api/productos/{id}` | Autenticado | Obtiene un producto, su categoría y, si existe, su imagen. |
| POST | `/api/productos` | Admin | Crea un producto. |
| PUT | `/api/productos/{id}` | Admin | Actualiza un producto. |
| DELETE | `/api/productos/{id}` | Admin | Elimina un producto. |
| POST | `/api/productos/{id}/imagen` | Admin | Sube o reemplaza la imagen del producto. |

Parámetros del listado: `buscar` por nombre, `categoriaId`, `ordenarPor` (`id`, `nombre`, `precio`, `stock`) y `direccion` (`asc`, `desc`).

La imagen se carga como `multipart/form-data`, con el campo `archivo`. Se aceptan PNG y JPG/JPEG de hasta 5 MB.

### Proveedores

Requests de Bruno: [Proveedores](https://github.com/juliancarola04/BrunoCollectionTP5/tree/main/Proveedores).

| Método | Ruta | Acceso | Descripción |
| --- | --- | --- | --- |
| GET | `/api/proveedores/admin` | Admin | Lista proveedores paginados. |
| GET | `/api/proveedores/admin/{id}` | Admin | Obtiene un proveedor. |
| POST | `/api/proveedores/admin/crear` | Admin | Crea un proveedor. |
| PUT | `/api/proveedores/admin/actualizar/{id}` | Admin | Actualiza un proveedor. |
| DELETE | `/api/proveedores/admin/dardebaja/{id}` | Admin | Da de baja lógica a un proveedor. |

Parámetros del listado: `eliminado`, `buscar` por razón social, `ordenarPor` (`id`, `razonSocial`) y `direccion` (`asc`, `desc`).

### Usuarios

Requests de Bruno: [Usuarios](https://github.com/juliancarola04/BrunoCollectionTP5/tree/main/Usuarios).

| Método | Ruta | Acceso | Descripción |
| --- | --- | --- | --- |
| GET | `/api/usuarios` | Admin | Lista usuarios paginados. |
| POST | `/api/usuarios/convertirenadministrador/{id}` | Admin | Otorga el rol de administrador. |
| POST | `/api/usuarios/quitaradministrador/{id}` | Admin | Quita el rol de administrador. |
| PUT | `/api/usuarios/actualizar` | Autenticado | Actualiza al usuario del token y devuelve un token nuevo. |
| DELETE | `/api/usuarios/dardebaja` | Autenticado | Da de baja lógica al usuario del token. |
| PUT | `/api/usuarios/admin/actualizar/{id}` | Admin | Actualiza cualquier usuario. |
| DELETE | `/api/usuarios/admin/dardebaja/{id}` | Admin | Da de baja lógica a cualquier usuario. |

Parámetros del listado: `esAdministrador` y `eliminado`, además del paginado común.

El body de actualización admite `username`, `email` y `password`.

### Ventas

Requests de Bruno: [Ventas](https://github.com/juliancarola04/BrunoCollectionTP5/tree/main/Ventas).

| Método | Ruta | Acceso | Descripción |
| --- | --- | --- | --- |
| GET | `/api/ventas` | Autenticado | Lista ventas paginadas. |
| GET | `/api/ventas/{id}` | Autenticado | Obtiene una venta con sus detalles. |
| POST | `/api/ventas` | Admin | Registra una venta y descuenta stock. |
| DELETE | `/api/ventas/{id}` | Admin | Anula una venta y repone stock. |

Parámetros del listado: `clienteId`, `anulada`, `buscar` por nombre del cliente, `ordenarPor` (`fecha`, `total`) y `direccion` (`asc`, `desc`).

### Ingresos

Requests de Bruno: [Ingresos](https://github.com/juliancarola04/BrunoCollectionTP5/tree/main/Ingresos).

| Método | Ruta | Acceso | Descripción |
| --- | --- | --- | --- |
| GET | `/api/ingresos` | Autenticado | Lista ingresos paginados. |
| GET | `/api/ingresos/{id}` | Autenticado | Obtiene un ingreso con sus detalles. |
| POST | `/api/ingresos` | Admin | Registra un ingreso y aumenta stock. |
| DELETE | `/api/ingresos/{id}` | Admin | Anula un ingreso si hay stock suficiente para revertirlo. |

Parámetros del listado: `proveedorId`, `anulado`, `buscar` por razón social del proveedor, `ordenarPor` (`fecha`, `total`) y `direccion` (`asc`, `desc`).

## Lógica de inventario

### Ingreso

Un ingreso representa una compra a un proveedor. Lo registra el administrador autenticado y guarda su identificador en el movimiento.

1. Debe incluir al menos un ítem.
2. Cada ítem debe tener cantidad y precio unitario mayores a cero.
3. No se permite repetir un producto dentro de la misma lista de ítems.
4. Se verifica que existan el proveedor y todos los productos.
5. Por cada detalle se aumenta el `Stock` del producto y se actualiza su `PrecioCompra` con el `precioUnitario` de ese ingreso.
6. El total del ingreso es la suma de `cantidad × precioUnitario` de sus detalles.

Al anularlo, la API primero comprueba que descontar las cantidades no deje stock negativo. Si puede revertirse, resta esas cantidades y marca el ingreso como `Anulado`. La anulación no restaura el `PrecioCompra` anterior.

### Venta

Una venta representa la salida de mercadería a un cliente. También la registra el administrador autenticado.

1. Debe incluir al menos un ítem y cada cantidad debe ser mayor a cero.
2. Se verifica que exista el cliente.
3. Si un producto se repite en el body, las cantidades se agrupan antes de validar el stock.
4. Se verifica que cada producto exista y tenga stock suficiente.
5. Se descuenta el stock y se guarda en el detalle el `PrecioVenta` vigente como `PrecioUnitario`.
6. El total se calcula con `cantidad × PrecioVenta` de cada producto al momento de crear la venta.

Al anular una venta, se repone la cantidad de cada detalle al stock y la venta queda marcada como `Anulada`.
