# 🚗 AUTORENT

Aplicación móvil de renta de autos desarrollada con .NET MAUI y Supabase.

## 📱 Características

- ✅ Autenticación de usuarios (Login/Registro)
- ✅ Sesión persistente
- ✅ Modo oscuro global
- ✅ Dos roles: Conductor y Propietario
- ✅ Publicar vehículos con fotos
- ✅ Ver vehículos disponibles
- ✅ Gestión de rentas
- ✅ Perfil de usuario editable

## 🛠️ Tecnologías

- **.NET MAUI** - Framework multiplataforma
- **Supabase** - Backend (PostgreSQL + Auth)
- **C#** - Lenguaje de programación
- **XAML** - Interfaz de usuario

## 📦 Estructura del Proyecto

```
AUTORENT/
├── Models/          # Modelos de datos
├── Services/        # Servicios (Auth, Theme)
├── Pages/           # Páginas de la aplicación
├── Resources/       # Recursos (imágenes, estilos)
└── Platforms/       # Configuración por plataforma
```

## 🚀 Instalación

### Requisitos previos
- Visual Studio 2022 con carga de trabajo .NET MAUI
- Android SDK
- Cuenta de Supabase

### Configuración

1. Clona el repositorio:
```bash
git clone https://github.com/jordi23-acosta/Autorent.git
```

2. Abre el proyecto en Visual Studio 2022

3. Configura tus credenciales de Supabase en `App.xaml.cs`:
```csharp
var supabaseUrl = "TU_URL_DE_SUPABASE";
var supabaseKey = "TU_KEY_DE_SUPABASE";
```

4. Ejecuta el proyecto en un emulador o dispositivo Android

## 📊 Base de Datos

El esquema de la base de datos se encuentra en `supabase_schema.sql` e incluye:
- Tabla `profiles` - Perfiles de usuario
- Tabla `vehicles` - Vehículos publicados
- Tabla `rentals` - Rentas activas e historial
- Tabla `favorites` - Vehículos favoritos
- Tabla `reviews` - Reseñas de vehículos

## 👥 Roles de Usuario

### Conductor
- Ver vehículos disponibles
- Rentar vehículos
- Ver historial de rentas
- Gestionar favoritos

### Propietario
- Publicar vehículos
- Gestionar mis vehículos
- Ver solicitudes de renta
- Ver ganancias

## 📸 Capturas de Pantalla

_(Agregar capturas de pantalla aquí)_

## 📄 Licencia

Este proyecto es de código abierto y está disponible bajo la licencia MIT.

## 👨‍💻 Autor

Desarrollado por Jordi Acosta
