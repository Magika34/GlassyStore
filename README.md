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

The application seeds the default roles at startup, but admin user creation is restricted to development or when explicitly enabled in configuration.

Seeding behavior

- By default seeding runs only when the app is running in the Development environment.
- To enable seeding in other environments, set the configuration value Seed:Enable = true.

Admin account

- Default admin email: admin@localhost (used in Development if no Seed:AdminEmail is provided)
- Default admin password in Development (for convenience): Admin123!
- For non-development environments you MUST provide a password via configuration: Seed:AdminPassword

Production guidance

- Do NOT enable seeding in production unless you intend to create the account there. Use secrets or environment variables to supply Seed:AdminPassword when needed.
- Remove or change the seeded admin credentials and disable Seed:Enable in production.

The bundled EmailService is a debug stub that writes to the debug output. Replace with a real SMTP or third-party provider for sending real emails.

---

## Author

Maggie