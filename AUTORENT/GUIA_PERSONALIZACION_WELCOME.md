# Guía de Personalización - Pantalla de Bienvenida (WelcomePage)

## 📱 Descripción General

La pantalla de bienvenida (WelcomePage) es un carrusel de 4 slides con **imágenes a pantalla completa** que se muestra la primera vez que el usuario abre la app. 

### ✨ Características Principales:
- **Imágenes a pantalla completa** con degradado oscuro para legibilidad
- **Auto-play**: Los slides cambian automáticamente cada 4 segundos
- **Loop infinito**: El carrusel vuelve al inicio después del último slide
- **Botones fijos**: "Iniciar sesión" y "Registrarse" siempre visibles
- **Textos dinámicos**: El título y descripción cambian con cada slide
- Después de verla, la app recuerda que ya fue vista y va directo al login

## 🎨 Cómo Personalizar las Pantallas

### Ubicación de los Archivos
- **Diseño visual**: `AUTORENT/WelcomePage.xaml`
- **Lógica y contenido**: `AUTORENT/WelcomePage.xaml.cs`

### Estructura Actual

El diseño tiene:
1. **Carrusel de imágenes a pantalla completa** (fondo)
2. **Degradado oscuro** sobre las imágenes para legibilidad
3. **Contenido superpuesto**:
   - Título (dinámico)
   - Descripción (dinámica)
   - Indicadores de página
   - Botones "Iniciar sesión" y "Registrarse"

---

## � Personalizar los Textos de los Slides

Edita el archivo `WelcomePage.xaml.cs` y modifica el array `OnboardingItems`:

```csharp
OnboardingItems = new ObservableCollection<OnboardingItem>
{
    new OnboardingItem
    {
        ImageSource = "onboarding_1.png",
        Title = "Encuentra. Reserva. Estaciona.",
        Description = "Localiza lugares disponibles cerca de ti en tiempo real."
    },
    new OnboardingItem
    {
        ImageSource = "onboarding_2.png",
        Title = "Renta tu auto ideal",
        Description = "Miles de vehículos disponibles con entrega a domicilio."
    },
    new OnboardingItem
    {
        ImageSource = "onboarding_3.png",
        Title = "Reserva en segundos",
        Description = "Proceso rápido y seguro desde tu celular."
    },
    new OnboardingItem
    {
        ImageSource = "onboarding_4.png",
        Title = "Gana dinero con tu auto",
        Description = "Renta tu vehículo y genera ingresos extra de forma segura."
    }
};
```

**Puedes cambiar:**
- `ImageSource`: Nombre del archivo de imagen
- `Title`: Título principal (grande y bold)
- `Description`: Texto descriptivo (más pequeño y ligero)

---

## ⏱️ Ajustar la Velocidad del Auto-Play

En `WelcomePage.xaml.cs`, busca la función `StartAutoPlay()`:

```csharp
private void StartAutoPlay()
{
    // Cambiar de slide cada 4 segundos
    _autoPlayTimer = new System.Threading.Timer((state) =>
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _currentIndex = (_currentIndex + 1) % OnboardingItems.Count;
            OnboardingCarousel.ScrollTo(_currentIndex, animate: true);
        });
    }, null, TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(4));
    //                              ↑ Cambia este número
}
```

**Velocidades recomendadas:**
- `3` segundos: Rápido
- `4` segundos: Normal (actual)
- `5-6` segundos: Lento (más tiempo para leer)

---

## � Desactivar el Auto-Play

Si prefieres que el usuario deslice manualmente:

1. En `WelcomePage.xaml.cs`, comenta o elimina esta línea en el constructor:
   ```csharp
   // StartAutoPlay();  // ← Comentar esta línea
   ```

2. También puedes desactivar el loop infinito en `WelcomePage.xaml`:
   ```xml
   <CarouselView x:Name="OnboardingCarousel"
                 Loop="False"  <!-- Cambiar a False -->
   ```

---

## 🎨 Personalización de Colores

### Colores Actuales:

**Degradado del fondo de las imágenes:**
```xml
<LinearGradientBrush StartPoint="0,0" EndPoint="0,1">
    <GradientStop Color="#60000000" Offset="0.0" />
    <GradientStop Color="#90000000" Offset="0.7" />
    <GradientStop Color="#D0000000" Offset="1.0" />
</LinearGradientBrush>
```

**Botón "Iniciar sesión" (transparente con borde):**
```xml
<Border Stroke="White" StrokeThickness="2" BackgroundColor="Transparent">
```

**Botón "Registrarse" (cyan/turquesa):**
```xml
<Border BackgroundColor="#00E5CC">
    <Button TextColor="#000000" />  <!-- Texto negro -->
</Border>
```

**Indicadores de página:**
```xml
<IndicatorView IndicatorColor="#60FFFFFF"           <!-- Inactivo: blanco semi-transparente -->
               SelectedIndicatorColor="#00E5CC"     <!-- Activo: cyan -->
```

### Cómo Cambiar los Colores:

**1. Color del botón "Registrarse":**
```xml
<!-- En WelcomePage.xaml, busca: -->
<Border BackgroundColor="#00E5CC">  <!-- Cambia este color -->
```

Colores sugeridos:
- `#00E5CC` - Cyan (actual)
- `#5B9FFF` - Azul
- `#FF6B6B` - Rojo coral
- `#4CAF50` - Verde
- `#9C27B0` - Morado

**2. Intensidad del degradado oscuro:**
```xml
<!-- Más oscuro (mejor legibilidad): -->
<GradientStop Color="#80000000" Offset="0.0" />
<GradientStop Color="#B0000000" Offset="0.7" />
<GradientStop Color="#E0000000" Offset="1.0" />

<!-- Más claro (imagen más visible): -->
<GradientStop Color="#40000000" Offset="0.0" />
<GradientStop Color="#70000000" Offset="0.7" />
<GradientStop Color="#A0000000" Offset="1.0" />
```

---

## 🔤 Personalización de Tipografía

### Tamaños Actuales:

**Título:**
```xml
<Label x:Name="TitleLabel"
       FontSize="42"              <!-- Tamaño grande -->
       FontAttributes="Bold"      <!-- Negrita -->
       CharacterSpacing="0.3"     <!-- Espaciado entre letras -->
       LineHeight="1.1" />        <!-- Altura de línea compacta -->
```

**Descripción:**
```xml
<Label x:Name="DescriptionLabel"
       FontSize="16"              <!-- Tamaño normal -->
       FontAttributes="None"      <!-- Sin negrita -->
       CharacterSpacing="0.2"     <!-- Menos espaciado -->
       LineHeight="1.4" />        <!-- Más espacio entre líneas -->
```

### Ajustar Tamaños:

**Para títulos más grandes:**
```xml
FontSize="48"  <!-- Muy grande -->
FontSize="42"  <!-- Grande (actual) -->
FontSize="36"  <!-- Mediano -->
```

**Para descripciones:**
```xml
FontSize="18"  <!-- Más grande -->
FontSize="16"  <!-- Normal (actual) -->
FontSize="14"  <!-- Más pequeño -->
```

---

## 🖼️ Agregar o Quitar Slides

### Agregar un Nuevo Slide:

En `WelcomePage.xaml.cs`, agrega un nuevo item al array:

```csharp
OnboardingItems = new ObservableCollection<OnboardingItem>
{
    // ... slides existentes ...
    
    new OnboardingItem
    {
        ImageSource = "onboarding_5.png",  // Nueva imagen
        Title = "Tu nuevo título",
        Description = "Tu nueva descripción"
    }
};
```

### Quitar un Slide:

Simplemente elimina el bloque correspondiente del array en `WelcomePage.xaml.cs`.

---

## 🎯 Personalizar los Botones

### Cambiar los Textos:

En `WelcomePage.xaml`:

```xml
<!-- Botón Iniciar sesión -->
<Button Text="Iniciar sesión"  <!-- Cambia aquí -->

<!-- Botón Registrarse -->
<Button Text="Registrarse"     <!-- Cambia aquí -->
```

### Cambiar el Estilo:

**Botón con degradado:**
```xml
<Border.Background>
    <LinearGradientBrush StartPoint="0,0" EndPoint="1,0">
        <GradientStop Color="#5B9FFF" Offset="0.0" />
        <GradientStop Color="#3D7FE6" Offset="1.0" />
    </LinearGradientBrush>
</Border.Background>
```

**Botón sólido:**
```xml
<Border BackgroundColor="#00E5CC">
```

**Botón con borde:**
```xml
<Border Stroke="White" 
        StrokeThickness="2" 
        BackgroundColor="Transparent">
```

---

## 🔄 Reiniciar el Onboarding (Para Pruebas)

Si quieres volver a ver la pantalla de bienvenida después de haberla visto:

1. **Opción 1: Desinstalar y reinstalar la app**

2. **Opción 2: Limpiar datos de la app**
   - Ve a Configuración → Apps → AUTORENT → Almacenamiento → Borrar datos

3. **Opción 3: Código (para desarrollo)**
   En `App.xaml.cs`, temporalmente cambia:
   ```csharp
   bool hasSeenOnboarding = Preferences.Get("HasSeenOnboarding", false);
   ```
   Por:
   ```csharp
   bool hasSeenOnboarding = false; // Siempre muestra el onboarding
   ```

---

## � Tips de Diseño

1. **Imágenes de alta calidad** - Usa imágenes de al menos 1920x1080px
2. **Contraste adecuado** - Asegúrate de que el texto sea legible sobre las imágenes
3. **Textos cortos** - Los usuarios no leen mucho en onboarding
4. **Máximo 4-5 slides** - Más de eso cansa al usuario
5. **Auto-play moderado** - 4-5 segundos es ideal
6. **Consistencia visual** - Usa los mismos colores y estilos en todos los slides

---

## 🚀 Después de Personalizar

1. Guarda los cambios en `WelcomePage.xaml` y `WelcomePage.xaml.cs`
2. Compila la app: `dotnet build AUTORENT/AUTORENT.csproj -f net10.0-android`
3. Despliega al emulador: `dotnet build AUTORENT/AUTORENT.csproj -t:Run -f net10.0-android`
4. Desinstala la app del emulador para ver el onboarding de nuevo

---

## 📝 Notas Importantes

- La app solo muestra el onboarding **la primera vez**
- Después va directo al login
- El estado se guarda con `Preferences.Set("HasSeenOnboarding", true)`
- Los slides cambian automáticamente cada 4 segundos
- El carrusel hace loop infinito (vuelve al inicio)
- Los botones "Iniciar sesión" y "Registrarse" están siempre visibles
- El auto-play se detiene cuando el usuario hace clic en cualquier botón

---

## 🎬 Comportamiento del Auto-Play

### Cómo Funciona:
1. El carrusel inicia en el slide 1
2. Cada 4 segundos, avanza automáticamente al siguiente
3. Cuando llega al último slide, vuelve al primero (loop infinito)
4. El usuario puede deslizar manualmente en cualquier momento
5. Al hacer clic en "Iniciar sesión" o "Registrarse", el auto-play se detiene

### Interacción Usuario:
- **Deslizar manualmente**: El auto-play continúa desde el nuevo slide
- **Hacer clic en botones**: El auto-play se detiene y navega a la siguiente pantalla
- **Indicadores**: Muestran en qué slide estás actualmente

---

¡Listo! Ahora tienes una pantalla de bienvenida moderna con imágenes a pantalla completa y auto-play. 🎨✨
