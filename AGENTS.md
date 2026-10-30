# AGENTS

Linee guida operative per agenti che lavorano su questo repository.

## Obiettivo del dominio

Implementare un notification provider email con scelta provider a runtime (Google, Microsoft, Custom) usando polimorfismo e configurazioni lette da SQL Server con Dapper.

## Vincoli architetturali

- NotifyProviderAPI deve contenere solo endpoint, binding request/response e orchestrazione HTTP.
- NotifyProviderApplication deve contenere tutta la logica applicativa.
- Usare CQRS con MediatR per i casi d'uso.
- Non accedere al database direttamente nel progetto API.

## Convenzioni

- Command nel path Features/<FeatureName>/.
- Handler nel medesimo path del command.
- Accesso dati via interfacce in Abstractions e implementazioni in Repositories.
- Provider email in Providers con factory dedicata.

## Pattern obbligatori

1. CQRS:
   - Ogni write operation deve essere un Command MediatR.
   - Le read operation complesse devono diventare Query MediatR.
2. Polimorfismo:
   - Ogni provider implementa IEmailProviderSender.
   - La risoluzione del provider avviene solo tramite factory.
3. Dapper:
   - Query SQL esplicite in repository.
   - Nessun ORM diverso da Dapper per il layer applicativo.

## Checklist prima del merge

- Build solution senza errori.
- Endpoint API non contiene business logic.
- Handler copre validazioni applicative principali.
- SQL e mapping Dapper coerenti con schema.
- Documentazione README aggiornata se cambia schema o flusso.
