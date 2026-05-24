# 🎯 Kiro Skills para AUTORENT

Este directorio contiene **skills** de Kiro - guías y mejores prácticas para mejorar el diseño y arquitectura de la aplicación AUTORENT.

## 📚 Skills Disponibles

### 1. **maui-design-principles.md**
Principios fundamentales de diseño para .NET MAUI:
- ✅ Optimización de performance
- ✅ Diseño responsivo
- ✅ Soporte para modo oscuro
- ✅ Accesibilidad
- ✅ Animaciones y transiciones
- ✅ Patrones de UI modernos

**Cuándo usar**: Al diseñar nuevas páginas o refactorizar UI existente.

### 2. **maui-ui-components.md**
Biblioteca completa de componentes UI con ejemplos:
- 🔘 Botones (estándar, outlined, icon)
- 📝 Inputs y formularios
- 🎴 Cards y tarjetas
- 📋 Listas y CollectionViews
- 🧭 Navegación y tabs
- 🏷️ Badges y chips
- ⏳ Progress indicators
- 💬 Dialogs y alerts
- 🔄 Switches y toggles
- ⭐ Rating components

**Cuándo usar**: Al implementar componentes específicos de UI.

### 3. **maui-mvvm-patterns.md**
Patrones de arquitectura MVVM completos:
- 🏗️ BaseViewModel implementation
- ⚡ RelayCommand y AsyncRelayCommand
- 🔗 Data binding patterns
- 🔄 Value converters
- 📱 Navigation patterns
- 💬 Messaging patterns
- 🧪 Unit testing ViewModels

**Cuándo usar**: Al crear nuevos ViewModels o refactorizar lógica de negocio.

### 4. **maui-layouts-responsive.md**
Layouts y diseño responsivo:
- 📐 Grid, StackLayout, FlexLayout, AbsoluteLayout
- 📱 Diseño adaptativo (OnIdiom, OnPlatform)
- 🔄 Layouts orientación-aware
- 🎨 Patrones comunes (cards, forms, dashboard)
- ⚡ Tips de performance
- 📏 Sistema de espaciado consistente

**Cuándo usar**: Al estructurar páginas y crear layouts complejos.

## 🚀 Cómo Usar las Skills

### En Kiro Chat
Simplemente menciona la skill que necesitas:

```
"Usa la skill de maui-design-principles para mejorar el LoginPage"
"Aplica los patrones de maui-ui-components para crear un mejor card"
"Refactoriza usando maui-mvvm-patterns"
```

### Activar Skills Manualmente
En Kiro, puedes activar skills usando el comando:
```
#maui-design-principles
```

### Aplicar a Tu Proyecto

1. **Para mejorar una página existente**:
   - Lee la skill relevante
   - Identifica patrones aplicables
   - Refactoriza gradualmente

2. **Para crear nueva funcionalidad**:
   - Consulta maui-mvvm-patterns para estructura
   - Usa maui-ui-components para componentes
   - Aplica maui-design-principles para polish

3. **Para optimizar performance**:
   - Revisa maui-design-principles (sección Performance)
   - Aplica maui-layouts-responsive (Layout Performance Tips)

## 🎨 Mejoras Recomendadas para AUTORENT

### Prioridad Alta
1. **LoginPage & RegisterPage**
   - Aplicar inputs con Material Design (maui-ui-components)
   - Mejorar botones con sombras y estados
   - Agregar validación visual

2. **MainPage (Lista de Vehículos)**
   - Usar cards mejorados (maui-ui-components)
   - Implementar skeleton loaders
   - Agregar pull-to-refresh

3. **ProfilePage**
   - Mejorar formulario con borders
   - Agregar secciones con cards
   - Implementar estados de guardado

### Prioridad Media
4. **RentalsPage**
   - Agrupar por estado con headers
   - Usar chips para estados
   - Agregar empty states

5. **MyVehiclesPage**
   - Dashboard con stats cards
   - Gráficos de ganancias
   - Swipe actions para editar/eliminar

### Prioridad Baja
6. **Animaciones y Transiciones**
   - Page transitions
   - Loading animations
   - Micro-interactions

## 📖 Recursos Adicionales

- [Microsoft .NET MAUI Docs](https://learn.microsoft.com/en-us/dotnet/maui/)
- [Material Design 3](https://m3.material.io/)
- [Syncfusion Components](https://www.syncfusion.com/maui-controls)

## 🔧 Configuración de Syncfusion

Ya tienes Syncfusion instalado en el proyecto. Consulta `SYNCFUSION_GUIDE.md` en la raíz del proyecto para ejemplos específicos de Syncfusion.

---

**Tip**: Estas skills están diseñadas para ser consultadas por Kiro automáticamente cuando trabajas en diseño y arquitectura. ¡Simplemente pide lo que necesitas!
