# Tablex API

API de **Tablex**, un programa de gestió de taules i dels comptes de cada taula pensat per a bars.

Permet portar el control de les taules del local, obrir comandes, afegir-hi productes i cobrar-les (fins i tot per parts) en efectiu o amb targeta.

## Funcionalitats

- **Autenticació** amb JWT i dos rols d'usuari:
  - `camarero`: obre comandes a les taules i hi afegeix, modifica o elimina línies.
  - `caja`: gestiona taules, productes i usuaris, cobra i tanca les comandes.
- **Taules (mesas)**: alta, edició i baixa, i consulta de la comanda oberta de cada taula.
- **Productes**: catàleg de productes classificats com a `comestible` o `bebida`.
- **Comandes (pedidos)**: una comanda oberta per taula, amb les seves línies de detall.
- **Pagaments**: cobrament de totes les línies o només d'algunes (per dividir el compte), amb mètode `efectivo` o `tarjeta`. Una comanda només es pot tancar quan no queda res pendent.
- **Errors uniformes**: totes les respostes d'error segueixen el mateix format (`ApiError`).

## Tecnologies

- .NET 8 / ASP.NET Core Web API
- Entity Framework Core amb SQL Server
- Autenticació JWT Bearer
- BCrypt per a les contrasenyes
- Swagger / OpenAPI

## Estructura del projecte

```
Controllers/   Endpoints de l'API
DTOs/          Objectes de petició i resposta
Data/          DbContext d'Entity Framework
Extensions/    Gestió d'errors, mapatges i utilitats
Lib/Consts/    Constants (rols, estats, tipus, missatges d'error)
Models/        Entitats de la base de dades
Services/      Lògica de negoci
```

## Posada en marxa

### Requisits

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server

### Configuració

Al fitxer `appsettings.json` (o `appsettings.Development.json`) cal indicar la cadena de connexió i la clau JWT:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Tablex;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "una-clau-secreta-llarga-de-com-a-minim-32-caracters",
    "Issuer": "TablexAPI",
    "Audience": "TablexClient",
    "ExpiresMinutes": 720
  }
}
```

> No pugis al repositori claus ni contrasenyes reals.

### Executar

```bash
dotnet restore
dotnet run
```

La documentació interactiva estarà disponible a `/swagger`. Per provar els endpoints protegits, fes login a `POST /api/auth/login` i enganxa el token al botó **Authorize**.

El fitxer `TablexAPI.http` conté peticions d'exemple per provar l'API des de VS Code o Visual Studio.

## Endpoints principals

| Mètode | Ruta | Rol | Descripció |
|---|---|---|---|
| POST | `/api/auth/login` | — | Iniciar sessió |
| GET | `/api/auth/me` | qualsevol | Usuari actual |
| GET | `/api/users` | caja | Llistar usuaris |
| POST | `/api/users` | — | Crear usuari |
| GET | `/api/mesas` | qualsevol | Llistar taules |
| POST / PUT / DELETE | `/api/mesas/{id}` | caja | Gestionar taules |
| GET | `/api/mesas/{mesaId}/pedido-abierto` | qualsevol | Comanda oberta de la taula |
| POST | `/api/mesas/{mesaId}/pedidos` | camarero | Obrir comanda |
| GET | `/api/productos` | qualsevol | Llistar productes |
| POST / PUT / DELETE | `/api/productos/{id}` | caja | Gestionar productes |
| GET | `/api/pedidos/{id}` | qualsevol | Veure comanda |
| POST | `/api/pedidos/{id}/detalles` | camarero | Afegir línia |
| PUT / DELETE | `/api/pedidos/{id}/detalles/{detalleId}` | camarero | Modificar / eliminar línia |
| GET | `/api/pedidos/{id}/pendiente` | qualsevol | Import pendient de pagament |
| GET | `/api/pedidos/{id}/pagos` | qualsevol | Pagaments de la comanda |
| POST | `/api/pedidos/{id}/pagos` | caja | Registrar pagament |
| POST | `/api/pedidos/{id}/cerrar` | caja | Tancar comanda |
