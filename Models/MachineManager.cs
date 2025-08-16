using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvyyanBackend.Models
{
    public class MachineManager : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string MachineName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Dia { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Gg { get; set; }

        public int Needle { get; set; }

        public int Feeder { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Rpm { get; set; }

        public int Slit { get; set; }

        [MaxLength(100)]
        public string? Constat { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal Efficiency { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
