# 🔧 Configuración de Supabase para AUTORENT

## Paso 1: Obtener tus credenciales de Supabase

1. Ve a tu proyecto en Supabase: https://supabase.com/dashboard
2. Selecciona tu proyecto AUTORENT
3. En el menú lateral, ve a **Settings** (⚙️) → **API**
4. Copia los siguientes valores:
   - **Project URL** (ejemplo: `https://tuproyecto.supabase.co`)
   - **anon public** key (la clave que dice "anon" o "public")

## Paso 2: Configurar las credenciales en la app

Abre el archivo `App.xaml.cs` y reemplaza estos valores:

```csharp
const string SUPABASE_URL = "TU_SUPABASE_URL_AQUI";
const string SUPABASE_KEY = "TU_SUPABASE_ANON_KEY_AQUI";
```

Por ejemplo:
```csharp
const string SUPABASE_URL = "https://abcdefghijk.supabase.co";
const string SUPABASE_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
```

## Paso 3: Verificar que el esquema esté ejecutado

Asegúrate de que ya ejecutaste el archivo `supabase_schema.sql` en el SQL Editor de Supabase:

1. Ve a **SQL Editor** en el menú lateral
2. Crea una nueva query
3. Copia y pega todo el contenido de `supabase_schema.sql`
4. Haz clic en **Run** o presiona `Ctrl + Enter`

## Paso 4: Probar la aplicación

1. Compila y ejecuta la app
2. Deberías ver la pantalla de Login
3. Haz clic en "Crear cuenta"
4. Registra un nuevo usuario
5. ¡Listo! Ya estás conectado a Supabase

## 🔒 Seguridad

**IMPORTANTE:** En una app de producción, NO debes poner las credenciales directamente en el código. Deberías usar:
- Variables de entorno
- Archivos de configuración no versionados
- Azure Key Vault o similar

Para desarrollo está bien, pero recuerda cambiar esto antes de publicar.

## ✅ Verificar que funciona

Después de registrar un usuario, puedes verificar en Supabase:

1. Ve a **Authentication** → **Users** - deberías ver tu usuario
2. Ve a **Table Editor** → **profiles** - deberías ver tu perfil

## 🐛 Problemas comunes

### Error: "Invalid API key"
- Verifica que copiaste la clave correcta (debe ser la "anon public")
- Asegúrate de que no haya espacios al inicio o final

### Error: "relation profiles does not exist"
- No ejecutaste el schema SQL
- Ve al SQL Editor y ejecuta `supabase_schema.sql`

### Error: "Email rate limit exceeded"
- Supabase limita los emails en el plan gratuito
- Espera unos minutos o usa otro email

### No puedo iniciar sesión
- Verifica que el email esté confirmado (revisa tu bandeja de entrada)
- En desarrollo, puedes desactivar la confirmación de email en:
  **Authentication** → **Settings** → **Email Auth** → Desactiva "Enable email confirmations"
