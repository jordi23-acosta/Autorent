# 📸 INSTRUCCIONES: Cómo Agregar las Imágenes al Onboarding

## ✅ PASO 1: Generar las Imágenes

1. Abre el archivo `PROMPTS_IMAGENES_ONBOARDING.md`
2. Usa cualquiera de las herramientas recomendadas:
   - **Midjourney** (mejor calidad, de pago)
   - **DALL-E 3** (muy bueno, ChatGPT Plus)
   - **Leonardo.ai** (gratis con límites)
   - **Stable Diffusion** (open source, gratis)
   - **Adobe Firefly** (integrado con Adobe)

3. Copia y pega los prompts para cada slide
4. Genera las 4 imágenes

---

## ✅ PASO 2: Descargar y Optimizar

1. **Descarga las imágenes** en alta resolución
2. **Optimiza el tamaño** (recomendado: usa [TinyPNG.com](https://tinypng.com))
   - Peso máximo recomendado: 500KB por imagen
   - Formato: JPG (mejor compresión) o PNG
   - Resolución mínima: 1080x1080px

---

## ✅ PASO 3: Renombrar los Archivos

Renombra las imágenes exactamente así:

```
onboarding_1.jpg  → Slide 1: "Consigue tu auto de renta aquí"
onboarding_2.jpg  → Slide 2: "Renta en 3 simples pasos"
onboarding_3.jpg  → Slide 3: "Gana dinero con tu auto"
onboarding_4.jpg  → Slide 4: "Comienza tu viaje"
```

**⚠️ IMPORTANTE**: Los nombres deben ser exactamente como se muestran arriba (minúsculas, con guión bajo).

---

## ✅ PASO 4: Copiar al Proyecto

1. Navega a la carpeta del proyecto: `AUTORENT/Resources/Images/`
2. Copia las 4 imágenes en esa carpeta
3. Verifica que los archivos estén ahí:
   ```
   AUTORENT/Resources/Images/onboarding_1.jpg
   AUTORENT/Resources/Images/onboarding_2.jpg
   AUTORENT/Resources/Images/onboarding_3.jpg
   AUTORENT/Resources/Images/onboarding_4.jpg
   ```

---

## ✅ PASO 5: Actualizar el Proyecto

**YA ESTÁ LISTO** ✨ - El código ya está preparado para usar las imágenes automáticamente.

Las imágenes se cargarán automáticamente porque:
- El archivo `.csproj` ya incluye `<MauiImage Include="Resources\Images\*" />`
- El código `WelcomePage.xaml.cs` ya tiene las rutas configuradas
- El diseño `WelcomePage.xaml` ya tiene el control `Image` listo

---

## ✅ PASO 6: Compilar y Probar

1. **Limpia el proyecto** (recomendado):
   ```bash
   dotnet clean
   ```

2. **Compila el proyecto**:
   ```bash
   dotnet build
   ```

3. **Ejecuta la app** en el emulador o dispositivo

4. **Verifica** que las imágenes se vean correctamente en cada slide del carousel

---

## 🎨 RESULTADO ESPERADO

Cada slide del onboarding mostrará:

```
┌─────────────────────────────────┐
│         AUTORENT                │  ← Logo fijo (no se mueve)
├─────────────────────────────────┤
│                                 │
│   [IMAGEN REALISTA]             │  ← Tu imagen generada
│                                 │
│   Título Grande y Bold          │  ← Texto blanco, 48px
│                                 │
│   Descripción del slide         │  ← Texto gris, 18px
│   en varias líneas              │
│                                 │
│   [Botón Comenzar/Omitir]       │  ← Según el slide
│                                 │
│   ● ○ ○ ○                       │  ← Indicadores
└─────────────────────────────────┘
```

---

## 🔧 TROUBLESHOOTING

### ❌ Las imágenes no se ven

**Solución 1**: Verifica los nombres de archivo
- Deben ser exactamente: `onboarding_1.jpg`, `onboarding_2.jpg`, etc.
- Todo en minúsculas
- Con guión bajo (no espacio ni guión)

**Solución 2**: Limpia y recompila
```bash
dotnet clean
dotnet build
```

**Solución 3**: Verifica la ruta
- Las imágenes deben estar en: `AUTORENT/Resources/Images/`
- NO en subcarpetas

**Solución 4**: Verifica el formato
- Usa JPG o PNG
- Evita formatos exóticos (WEBP, AVIF, etc.)

### ❌ Las imágenes se ven pixeladas

**Solución**: Usa imágenes de mayor resolución
- Mínimo: 1080x1080px
- Recomendado: 1920x1920px
- No comprimas demasiado (calidad JPG mínima: 80%)

### ❌ Las imágenes tardan en cargar

**Solución**: Optimiza el tamaño de archivo
- Usa [TinyPNG.com](https://tinypng.com) para comprimir
- Peso máximo recomendado: 500KB por imagen
- Considera usar JPG en lugar de PNG

---

## 📝 NOTAS ADICIONALES

### Cambiar las Imágenes Después

Si quieres cambiar las imágenes más tarde:

1. Reemplaza los archivos en `AUTORENT/Resources/Images/`
2. Mantén los mismos nombres
3. Limpia y recompila el proyecto

### Usar Diferentes Formatos

El código soporta tanto JPG como PNG:
- **JPG**: Mejor compresión, archivos más pequeños (recomendado)
- **PNG**: Mejor calidad, soporta transparencia (más pesado)

Si usas PNG, simplemente cambia la extensión en el código:
```csharp
ImageSource = "onboarding_1.png"  // En lugar de .jpg
```

### Agregar Más Slides

Si quieres agregar un 5to slide:

1. Genera la imagen: `onboarding_5.jpg`
2. Cópiala a `Resources/Images/`
3. Agrega un nuevo `OnboardingItem` en `WelcomePage.xaml.cs`
4. Actualiza `IsLastSlide` en el slide 4 a `false`
5. Marca el slide 5 como `IsLastSlide = true`

---

## ✅ CHECKLIST FINAL

Antes de compilar, verifica:

- [ ] Las 4 imágenes están generadas
- [ ] Las imágenes están optimizadas (< 500KB cada una)
- [ ] Los nombres son correctos: `onboarding_1.jpg`, `onboarding_2.jpg`, etc.
- [ ] Las imágenes están en `AUTORENT/Resources/Images/`
- [ ] Has limpiado el proyecto (`dotnet clean`)
- [ ] Estás listo para compilar (`dotnet build`)

---

¡Listo! Una vez que tengas las imágenes, solo cópialas a la carpeta y compila. Todo lo demás ya está configurado. 🚗✨
