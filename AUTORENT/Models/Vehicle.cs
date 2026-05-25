using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AUTORENT.Models
{
    public enum VehicleCategory
    {
        Economic,
        SUV,
        Luxury,
        Van
    }

    public enum VehicleStatus
    {
        Available,
        Rented,
        Maintenance
    }

    [Table("vehicles")]
    public class Vehicle : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; } = string.Empty;

        [Column("owner_id")]
        public string OwnerId { get; set; } = string.Empty;

        [Column("brand")]
        public string Brand { get; set; } = string.Empty;

        [Column("model")]
        public string Model { get; set; } = string.Empty;

        [Column("year")]
        public int Year { get; set; }

        [Column("color")]
        public string? Color { get; set; }

        [Column("plate")]
        public string? Plate { get; set; }

        [Column("price_per_day")]
        public decimal PricePerDay { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("location")]
        public string? Location { get; set; }

        [Column("latitude")]
        public double? Latitude { get; set; }

        [Column("longitude")]
        public double? Longitude { get; set; }

        [Column("image_url")]
        public string? ImageUrl { get; set; }

        [Column("is_available")]
        public bool IsAvailable { get; set; } = true;

        [Column("seats")]
        public int Seats { get; set; } = 5;

        [Column("transmission")]
        public string Transmission { get; set; } = "automatico";

        [Column("fuel_type")]
        public string FuelType { get; set; } = "gasolina";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Propiedades helper
        public bool IsAutomatic => Transmission == "automatico";
        
        public VehicleStatus Status => IsAvailable ? VehicleStatus.Available : VehicleStatus.Rented;

        public string DisplayName => $"{Brand} {Model} {Year}";
        
        public string TransmissionDisplay => IsAutomatic ? "Automático" : "Manual";
    }
}
