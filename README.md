# GlassyStore

## Overview

GlassyStore is an ASP.NET Core MVC eyewear e-commerce platform.

Customers can:

- Browse eyewear
- Select lens options
- Enter prescription details
- Place orders

Administrators can:

- Manage products
- Manage lens options
- Update order status

---

## Technologies

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- AutoMapper
- Bootstrap
- ASP.NET Identity

---

## Features

- CRUD Operations
- Repository Pattern
- Service Layer
- AutoMapper
- ViewModels
- Role-Based Authorization
- Email Notifications
- Concurrency (RowVersion)

---

## Database

Run:

```
Add-Migration InitialCreate
Update-Database
```

---

## Default Roles

Administrator

Customer

---

The application now seeds the default roles at startup and creates a local development administrator account.

- Admin email: admin@localhost
- Admin password: Admin123!

Note: This admin account is created with EmailConfirmed=true to allow immediate login in local/dev environments. Change or remove this behavior for production.

The bundled EmailService is a debug stub that writes to the debug output. Replace with a real SMTP or third-party provider for sending real emails.

---

## Author

Maggie