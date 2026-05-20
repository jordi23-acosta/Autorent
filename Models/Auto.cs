namespace AutoRent.Models;

public class Auto
{
    public int Id { get; set; }
    public string Marca { get; set; }
    public string Modelo { get; set; }
    public int Ano { get; set; }
    public string Tipo { get; set; }
    public decimal PrecioHora { get; set; }
    public decimal PrecioDia { get; set; }
    public bool Disponible { get; set; }
    public string ImagenUrl { get; set; }
}
