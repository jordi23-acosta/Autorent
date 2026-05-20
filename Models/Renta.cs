using System;

namespace AutoRent.Models;

public class Renta
{
    public int Id { get; set; }
    public string UsuarioId { get; set; }
    public int AutoId { get; set; }
    public DateTime FechaInicio { get; set; }
    public int Duracion { get; set; } // en horas o dias según TipoRenta
    public string TipoRenta { get; set; } // "hora" o "dia"
    public decimal Total { get; set; }
    public string Estado { get; set; } // "activa", "finalizada"
}
