# NotifyProvider API - Endpoints

Base path: `/api/notifications`

## 1. Invia notifica

- **Metodo**: `POST`
- **Path**: `/api/notifications/send`
- **Descrizione**: Invia una notifica email usando il provider configurato per la notification key.

### Request Body

```json
{
  "notificationKey": "string",
  "siteUrl": "string",
  "subject": "string",
  "body": "string",
  "isBodyHtml": true
}
```

### Campi richiesti

- `notificationKey` (string, obbligatorio)
- `siteUrl` (string, obbligatorio)
- `subject` (string, obbligatorio)
- `body` (string, obbligatorio)
- `isBodyHtml` (bool, opzionale)

### Response 200 OK

```json
{
  "notificationKey": "string",
  "provider": "Google|Microsoft|Custom",
  "recipientCount": 0,
  "sentAtUtc": "2026-08-09T10:30:00Z"
}
```

### Altri codici di risposta

- `400 Bad Request`
  - Se mancano `notificationKey`, `siteUrl`, `subject` o `body`
  - Oppure in caso di `InvalidOperationException`
- `403 Forbidden`
  - In caso di `UnauthorizedAccessException`
- `500 Internal Server Error`
  - In caso di errore inatteso

### Esempio cURL

```bash
curl -X POST "http://localhost:5000/api/notifications/send" \
  -H "Content-Type: application/json" \
  -d '{
    "notificationKey": "ORDERS",
    "siteUrl": "https://mysite.example",
    "subject": "Nuovo ordine",
    "body": "Hai ricevuto un nuovo ordine.",
    "isBodyHtml": false
  }'
```

## 2. Validazione accesso sito

- **Metodo**: `POST`
- **Path**: `/api/notifications/validate-site`
- **Descrizione**: Verifica se una `notificationKey` è autorizzata per un determinato `siteUrl`.

### Request Body

```json
{
  "notificationKey": "string",
  "siteUrl": "string"
}
```

### Campi richiesti

- `notificationKey` (string, obbligatorio)
- `siteUrl` (string, obbligatorio)

### Response 200 OK

```json
{
  "notificationKey": "string",
  "siteUrl": "string",
  "isAllowed": true
}
```

### Altri codici di risposta

- `400 Bad Request`
  - Se mancano `notificationKey` o `siteUrl`
  - Oppure in caso di `InvalidOperationException`

### Esempio cURL

```bash
curl -X POST "http://localhost:5000/api/notifications/validate-site" \
  -H "Content-Type: application/json" \
  -d '{
    "notificationKey": "ORDERS",
    "siteUrl": "https://mysite.example"
  }'
```
