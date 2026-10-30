# Notify Provider

Progetto .NET 8 con architettura a due layer:

- NotifyProviderAPI: espone endpoint HTTP.
- NotifyProviderApplication: contiene CQRS, MediatR, logica applicativa, polimorfismo provider e accesso dati con Dapper.

## Architettura

### API Layer

Responsabilita:

- Validazione input HTTP di base.
- Invio command al layer applicativo tramite MediatR.
- Serializzazione response.

Endpoint principale:

- POST /api/notifications/send

Payload:

```json
{
  "notificationKey": "orders-confirmation",
  "siteUrl": "https://shop.example.com",
  "subject": "Nuovo ordine",
  "body": "Contenuto della mail",
  "isBodyHtml": false
}
```

### Application Layer

Pattern adottati:

- CQRS con MediatR.
- Polimorfismo via interfaccia IEmailProviderSender.
- Repository pattern con Dapper.

Flusso command:

1. NotificationsController invia SendNotificationCommand.
2. SendNotificationCommandHandler valida il sito chiamante leggendo la allowlist dal database.
3. SendNotificationCommandHandler legge configurazione e destinatari da SQL Server tramite INotificationProviderRepository.
4. EmailProviderFactory risolve il sender corretto (Google, Microsoft, Custom).
5. Il sender SMTP invia la mail.

## Provider Polimorfici

Provider implementati:

- GoogleEmailProviderSender
- MicrosoftEmailProviderSender
- CustomEmailProviderSender

Tutti implementano IEmailProviderSender tramite base class condivisa SMTP.

## Database SQL Server

La stringa di connessione e letta da:

- ConnectionStrings:NotifyProviderDb

Script pronti all'uso:

- database/001_create_notifyprovider_db.sql
- database/002_seed_notifyprovider_db.sql

Esecuzione rapida:

```bash
sqlcmd -S localhost -E -i database/001_create_notifyprovider_db.sql
sqlcmd -S localhost -E -i database/002_seed_notifyprovider_db.sql
```

Tabelle attese:

```sql
CREATE TABLE dbo.NotificationProviderConfigurations (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NotificationKey NVARCHAR(100) NOT NULL UNIQUE,
    ProviderType INT NOT NULL, -- 1=Google, 2=Microsoft, 3=Custom
    SmtpHost NVARCHAR(255) NOT NULL,
    SmtpPort INT NOT NULL,
    UseSsl BIT NOT NULL,
    Username NVARCHAR(255) NOT NULL,
    Password NVARCHAR(255) NOT NULL,
    FromAddress NVARCHAR(255) NOT NULL,
    FromName NVARCHAR(255) NOT NULL
);

CREATE TABLE dbo.NotificationRecipients (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NotificationKey NVARCHAR(100) NOT NULL,
    EmailAddress NVARCHAR(255) NOT NULL,
    CONSTRAINT FK_NotificationRecipients_NotificationProviderConfigurations
        FOREIGN KEY (NotificationKey)
        REFERENCES dbo.NotificationProviderConfigurations(NotificationKey)
);

CREATE TABLE dbo.NotificationAllowedSites (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NotificationKey NVARCHAR(100) NOT NULL,
    Host NVARCHAR(255) NOT NULL, -- es: example.com
    AllowSubdomains BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_NotificationAllowedSites_NotificationProviderConfigurations
        FOREIGN KEY (NotificationKey)
        REFERENCES dbo.NotificationProviderConfigurations(NotificationKey)
);

CREATE UNIQUE INDEX UX_NotificationAllowedSites_KeyHost
    ON dbo.NotificationAllowedSites(NotificationKey, Host);
```

## Endpoint validazione siti

- POST /api/notifications/validate-site

Payload:

```json
{
  "notificationKey": "orders-confirmation",
  "siteUrl": "https://shop.example.com"
}
```

Response:

```json
{
  "notificationKey": "orders-confirmation",
  "siteUrl": "https://shop.example.com",
  "isAllowed": true
}
```

## Matching URL: perche non usare contains

`contains` non e consigliato per sicurezza: puo generare falsi positivi (es. `evil-example.com` contiene `example.com`).

Implementazione corrente:

- Match esatto su host (`example.com`).
- Match sottodomini solo se `AllowSubdomains = 1` (es. `shop.example.com`).

Questo approccio e piu sicuro e controllabile.

## Avvio

1. Copia `.env.example` in `.env` (ignorato da git) e imposta `ConnectionStrings__NotifyProviderDb`. Mai committare `.env` o la connection string in appsettings.
2. Crea le tabelle SQL Server indicate sopra.
3. Avvia:

```bash
dotnet build NotifyProvider.sln
dotnet run --project NotifyProviderAPI
```

Swagger disponibile in ambiente Development.

## Estensioni future consigliate

- Gestione segreti in Azure Key Vault o Secret Manager.
- Retry policy e circuit breaker per provider esterni.
- Logging strutturato per audit notifiche.
- Query side CQRS separata per storico invii.
