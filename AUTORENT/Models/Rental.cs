using Newtonsoft.Json;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace AUTORENT.Models
{
    public enum RentalStatus
    {
        Pending,
        Active,
        Completed,
        Cancelled
    }

    [Table("rentals")]
    public class Rental : BaseModel
    {
        [PrimaryKey("id", false)]
        public string Id { get; set; } = string.Empty;

        [Column("vehicle_id")]
        public string VehicleId { get; set; } = string.Empty;

        [Column("renter_id")]
        public string RenterId { get; set; } = string.Empty;

        [Column("owner_id")]
        public string OwnerId { get; set; } = string.Empty;

        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Column("total_price")]
        public decimal TotalPrice { get; set; }

        [Column("status")]
        public string StatusString { get; set; } = "pendiente";

        [Column("pickup_location")]
        public string? PickupLocation { get; set; }

        [Column("notes")]
        public string? Notes { get; set; }

        [Column("payment_method")]
        public string PaymentMethod { get; set; } = "efectivo";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Propiedades de navegación (no mapeadas a la BD)
        [JsonIgnore]
        public Vehicle? Vehicle { get; set; }

        [JsonIgnore]
        public Profile? Renter { get; set; }

        [JsonIgnore]
        public Profile? Owner { get; set; }

        // Propiedad helper para el status (no se envía a la BD)
        [JsonIgnore]
        public RentalStatus Status
        {
            get => StatusString switch
            {
                "pendiente" => RentalStatus.Pending,
                "confirmada" => RentalStatus.Active,
                "activa" => RentalStatus.Active,
                "completada" => RentalStatus.Completed,
                "cancelada" => RentalStatus.Cancelled,
                _ => RentalStatus.Pending
            };
            set => StatusString = value switch
            {
                RentalStatus.Pending => "pendiente",
                RentalStatus.Active => "activa",
                RentalStatus.Completed => "completada",
                RentalStatus.Cancelled => "cancelada",
                _ => "pendiente"
            };
        }
    }
}
