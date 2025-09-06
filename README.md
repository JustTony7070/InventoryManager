# Inventory Manager

Applicazione desktop WPF per la gestione dell'inventario con supporto per database MySQL e archiviazione locale CSV.

### Gestione Dati

- **Prodotti**: ID, nome, quantità, prezzo, codice univoco
- **Ordini**: Nome cliente, prodotti con quantità, codice, stato
- **Ricerca**: Filtri per tutti i campi con ricerca case-insensitive

### Modalità di Archiviazione

- **MySQL**: Connessione con auto-sync
- **CSV Locale**: File di default o caricamento esterno
- **Sincronizzazione**: Switch automatico tra modalità

### Operazioni

- **CRUD**: Creazione, lettura, aggiornamento, eliminazione
- **Validazione**: Controllo unicità codici, formato prezzi, campi obbligatori
- **Export/Import**: Esportazione CSV e caricamento file esterni
- **Backup**: Salvataggio automatico delle modifiche

## Tecnologie

- **.NET 8.0** - Framework
- **WPF** - Interfaccia utente
- **MySQL.Data 9.2.0** - Connettore database
- **CsvHelper 33.0.1** - Gestione CSV
