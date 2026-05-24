-- =============================================
-- AUTORENT - Migración del trigger de registro
-- Para que guarde el teléfono también
-- Ejecutar en Supabase SQL Editor
-- =============================================

-- Eliminar trigger y función anterior
DROP TRIGGER IF EXISTS on_auth_user_created ON auth.users;
DROP FUNCTION IF EXISTS public.handle_new_user();

-- Crear nueva función que guarda más datos del metadata
CREATE OR REPLACE FUNCTION public.handle_new_user()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO public.profiles (id, email, full_name, phone, user_type)
    VALUES (
        NEW.id,
        NEW.email,
        COALESCE(NEW.raw_user_meta_data->>'full_name', 'Usuario'),
        NEW.raw_user_meta_data->>'phone',
        COALESCE(NEW.raw_user_meta_data->>'user_type', 'conductor')
    );
    RETURN NEW;
END;
$$ LANGUAGE plpgsql SECURITY DEFINER;

-- Recrear trigger
CREATE TRIGGER on_auth_user_created
    AFTER INSERT ON auth.users
    FOR EACH ROW EXECUTE FUNCTION public.handle_new_user();

-- Verificar que se creó correctamente
SELECT proname FROM pg_proc WHERE proname = 'handle_new_user';
