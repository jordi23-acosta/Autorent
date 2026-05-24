-- =============================================
-- AUTORENT - Migración para método de pago
-- Ejecutar en Supabase SQL Editor
-- =============================================

-- Agregar columna de método de pago a la tabla rentals
ALTER TABLE rentals 
ADD COLUMN IF NOT EXISTS payment_method TEXT DEFAULT 'efectivo' 
CHECK (payment_method IN ('efectivo', 'transferencia'));

-- Agregar comentario explicativo
COMMENT ON COLUMN rentals.payment_method IS 'Método de pago: efectivo (al entregar el auto) o transferencia bancaria';

-- Verificar que se agregó correctamente
SELECT column_name, data_type, column_default 
FROM information_schema.columns 
WHERE table_name = 'rentals' AND column_name = 'payment_method';
