# 🚗 AUTORENT

Aplicación móvil de renta de autos desarrollada con .NET MAUI y Supabase, implementando arquitectura MVVM completa.

## 📱 Características

- ✅ Autenticación de usuarios (Login/Registro)
- ✅ Sesión persistente
- ✅ Modo oscuro global
- ✅ Dos roles: Conductor y Propietario
- ✅ Publicar vehículos con fotos (cámara/galería)
- ✅ Ver vehículos disponibles con filtros
- ✅ Gestión de rentas (solicitar, aprobar, rechazar)
- ✅ Perfil de usuario editable
- ✅ Página de bienvenida con carrusel automático
- ✅ Estadísticas para propietarios
- ✅ Arquitectura MVVM completa

## 🏗️ Arquitectura

Este proyecto implementa el patrón **MVVM (Model-View-ViewModel)** de forma completa:

### ViewModels
- `BaseViewModel` - Clase base con INotifyPropertyChanged
- `LoginViewModel` - Lógica de autenticación
- `RegisterViewModel` - Lógica de registro con validación
- `ProfileViewModel` - Gestión de perfil y modo oscuro
- `MainViewModel` - Listado y filtrado de vehículos
- `AddVehicleViewModel` - Publicación de vehículos
- `RentalsViewModel` - Gestión de rentas
- `MyVehiclesViewModel` - Vehículos del propietario
- `WelcomeViewModel` - Onboarding con carrusel

### Commands
- `RelayCommand` - Comandos síncronos
- `RelayCommand<T>` - Comandos síncronos con parámetro
- `AsyncRelayCommand` - Comandos asíncronos

### Converters
- `BusyTextConverter` - Convierte estado ocupado a texto
- `InvertedBoolConverter` - Invierte valores booleanos

## 🛠️ Tecnologías

- **.NET MAUI 10** - Framework multiplataforma
- **Supabase** - Backend (PostgreSQL + Auth + Storage)
- **C# 12** - Lenguaje de programación
- **XAML** - Interfaz de usuario con bindings
- **MVVM Pattern** - Arquitectura de software

## 📦 Estructura del Proyecto

```
AUTORENT/
├── Models/              # Modelos de datos (User, Vehicle, Rental, Profile)
├── ViewModels/          # ViewModels con lógica de negocio
│   ├── BaseViewModel.cs
│   ├── RelayCommand.cs
│   └── [8 ViewModels específicos]
├── Converters/          # Convertidores de valores para bindings
├── Services/            # Servicios (Auth, Theme)
├── Pages/               # Vistas XAML con bindings
│   ├── LoginPage.xaml
│   ├── RegisterPage.xaml
│   ├── MainPage.xaml
│   ├── ProfilePage.xaml
│   ├── AddVehiclePage.xaml
│   ├── RentalsPage.xaml
│   ├── MyVehiclesPage.xaml
│   └── WelcomePage.xaml
├── Resources/           # Recursos (imágenes, estilos, fuentes)
└── Platforms/           # Configuración por plataforma
    └── Android/
        └── AndroidManifest.xml
```

## 🚀 Instalación

### Requisitos previos
- Visual Studio 2022 (17.8+) con carga de trabajo .NET MAUI
- .NET 10 SDK
- Android SDK (API 24-36)
- Cuenta de Supabase

### Configuración

1. Clona el repositorio:
```bash
git clone https://github.com/jordi23-acosta/Autorent.git
cd Autorent
```

2. Abre el proyecto en Visual Studio 2022

3. Configura tus credenciales de Supabase en `App.xaml.cs`:
```csharp
var supabaseUrl = "TU_URL_DE_SUPABASE";
var supabaseKey = "TU_KEY_DE_SUPABASE";
```

4. Restaura los paquetes NuGet:
```bash
dotnet restore
```

5. Ejecuta el proyecto en un emulador o dispositivo Android

### Generar APK para distribución

```bash
dotnet clean
dotnet publish -f net10.0-android -c Release
```

El APK firmado se generará en:
```
AUTORENT/bin/Release/net10.0-android/com.companyname.autorent-Signed.apk
```

## 📱 Compatibilidad

- **Mínimo**: Android 7.0 (API 24)
- **Target**: Android 15 (API 36)
- **Framework**: .NET 10

## 📊 Base de Datos

El esquema de la base de datos se encuentra en `supabase_schema.sql` e incluye:

### Tablas
- **profiles** - Perfiles de usuario con rol (conductor/propietario)
- **vehicles** - Vehículos publicados con categorías y precios
- **rentals** - Rentas con estados (pendiente, aprobada, rechazada, completada)
- **favorites** - Vehículos favoritos de usuarios
- **reviews** - Reseñas y calificaciones de vehículos

### Políticas RLS (Row Level Security)
- Usuarios solo pueden ver sus propios datos sensibles
- Propietarios pueden gestionar sus vehículos
- Sistema de permisos basado en roles

## 👥 Roles de Usuario

### 🚗 Conductor
- Ver catálogo de vehículos disponibles
- Filtrar por categoría y búsqueda
- Solicitar renta de vehículos
- Ver historial de rentas
- Gestionar vehículos favoritos
- Dejar reseñas

### 🏠 Propietario
- Publicar vehículos con fotos
- Gestionar inventario de vehículos
- Aprobar/rechazar solicitudes de renta
- Ver estadísticas (vehículos activos, rentas, ganancias)
- Gestionar disponibilidad

## 🎨 Características de UI/UX

- **Modo Oscuro**: Tema claro/oscuro persistente
- **Navegación Intuitiva**: Shell navigation con tabs
- **Carrusel de Bienvenida**: Onboarding con auto-play
- **Validación en Tiempo Real**: Feedback inmediato en formularios
- **Estados de Carga**: Indicadores visuales durante operaciones
- **Diseño Responsivo**: Adaptable a diferentes tamaños de pantalla
- **Iconos Emoji**: Representación visual de categorías de vehículos

## 📸 Capturas de Pantalla

_(Agregar capturas de pantalla aquí)_

## 🔧 Desarrollo

### Patrón MVVM Implementado

Todas las páginas siguen el patrón MVVM:

1. **Model**: Clases de datos en `Models/`
2. **View**: Archivos XAML con bindings en `Pages/`
3. **ViewModel**: Lógica de negocio en `ViewModels/`

Ejemplo de binding:
```xml
<Button Text="Iniciar Sesión" 
        Command="{Binding LoginCommand}"
        IsEnabled="{Binding IsBusy, Converter={StaticResource InvertedBoolConverter}}" />
```

### Comandos Asíncronos

Los ViewModels usan `AsyncRelayCommand` para operaciones asíncronas:

```csharp
public ICommand LoginCommand { get; }

public LoginViewModel()
{
    LoginCommand = new AsyncRelayCommand(LoginAsync);
}

private async Task LoginAsync()
{
    IsBusy = true;
    try
    {
        // Lógica de login
    }
    finally
    {
        IsBusy = false;
    }
}
```

## 🐛 Solución de Problemas

### Error de compilación XA1006
Si ves el warning sobre targetSdkVersion, verifica que `AndroidManifest.xml` tenga:
```xml
<uses-sdk android:minSdkVersion="24" android:targetSdkVersion="36" />
```

### App incompatible en dispositivo
- Verifica que el dispositivo tenga Android 7.0 o superior
- Habilita "Instalar desde fuentes desconocidas"
- Asegúrate de usar el APK firmado de la carpeta Release

## 📄 Licencia

Este proyecto es de código abierto y está disponible bajo la licencia MIT.

## 👨‍💻 Autor

Desarrollado por **Jordi Acosta**

---

⭐ Si te gusta este proyecto, dale una estrella en GitHub!
