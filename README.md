# Inventory Manager

WPF desktop application for inventory management with support for a MySQL database and local CSV storage.

### Data Management

- **Products**: ID, name, quantity, price, unique code  
- **Orders**: Customer name, products with quantities, code, status  
- **Search**: Filters on all fields with case-insensitive search  

### Storage Modes

- **MySQL**: Connection with auto-sync  
- **Local CSV**: Default file or external file loading  
- **Synchronization**: Automatic switching between modes  

### Operations

- **CRUD**: Create, read, update, delete  
- **Validation**: Code uniqueness, price format, required fields  
- **Export/Import**: CSV export and external file loading  
- **Backup**: Automatic saving of changes  

## Technologies

- **.NET 8.0** – Framework  
- **WPF** – User interface  
- **MySQL.Data 9.2.0** – Database connector  
- **CsvHelper 33.0.1** – CSV handling  
