# AspNetCore.AutoServiceRegistration

[![NuGet version (AutoServiceRegistration.AspNetCore)](https://img.shields.io/nuget/v/AutoServiceRegistration.AspNetCore)](https://www.nuget.org/packages/AutoServiceRegistration.AspNetCore)
[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/ruyut/AutoServiceRegistration.AspNetCore/publish.yml)](https://github.com/ruyut/AutoServiceRegistration.AspNetCore/actions/workflows/publish.yml)

## Overview

AspNetCore.AutoServiceRegistration is a library for automatically registering services in ASP.NET Core applications.
It simplifies dependency injection setup by scanning all loaded assemblies for classes that implement specific marker interfaces, and then registering them with the corresponding service lifetime.

For more details, visit the related blog post:
[AutoServiceRegistration.AspNetCore Example](https://www.ruyut.com/2025/02/AutoServiceRegistration.AspNetCore.html)
(written in Traditional Chinese).

## Features

- **Automatic Service Registration**: Scans assemblies to automatically register services that implement designated interfaces.
- **Lifetime Management**: Supports Singleton, Scoped, and Transient service lifetimes through marker interfaces:
   - `ISingletonService` for Singleton services.
   - `IScopedService` for Scoped services.
   - `ITransientService` for Transient services.
- **Ease of Use**: Simply add the extension method in your service configuration to enable automatic registration.

## Installation

Install the package via .NET CLI:

```bash
dotnet add package AutoServiceRegistration.AspNetCore
```

Alternatively, add a project reference to your ASP.NET Core application.

## Usage

1. **Register Services**
   
   Add the auto service registration extension in your service configuration:

   ```csharp
   // add using directive
   using AutoServiceRegistration.AspNetCore;
   
   var builder = WebApplication.CreateBuilder(args);
   
   // register service
   builder.Services.AddRegisterServices();
   
   var app = builder.Build();
   ```

2. **Implement the Service Class**

   Create a class that implements your service interface. Since it implements a marker interface, it will be automatically registered:
    
    ```csharp
    public interface IMyService : IScopedService
    {
    }
    ```
   
    ```csharp
    public class MyService : IMyService
    {
    }
    ```

    ```csharp
    [ApiController]
    [Route("users")]
    public class MyController : ControllerBase
    {
        private readonly IMyService _myService;

        public MyController(IMyService myService)
        {
            _myService = myService;
        }
    }
    ```

   In this implementation, the service class MyService implements the marker interface IScopedService, which signals the auto registration mechanism to register it with a scoped lifetime.
   This means that whenever a dependency on IMyService is requested (for instance, in the MyController constructor), the dependency injection container will provide an instance of MyService with the appropriate lifecycle management automatically.

## How It Works
The `AddRegisterServices` extension method:
- Uses reflection to scan all assemblies loaded in the current AppDomain.
- Filters out non-abstract classes that implement the specified marker interfaces.
- Registers each discovered service with the dependency injection container using the corresponding lifetime (Singleton, Scoped, or Transient).
