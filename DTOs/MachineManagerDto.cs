namespace AvyyanBackend.DTOs
{
    public class MachineManagerDto
    {
        public int Id { get; set; }
        public string MachineName { get; set; } = string.Empty;
        public decimal Dia { get; set; }
        public decimal Gg { get; set; }
        public int Needle { get; set; }
        public int Feeder { get; set; }
        public decimal Rpm { get; set; }
        public decimal? Constat { get; set; }
        public decimal Efficiency { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateMachineManagerDto
    {
        public string MachineName { get; set; } = string.Empty;
        public decimal Dia { get; set; }
        public decimal Gg { get; set; }
        public int Needle { get; set; }
        public int Feeder { get; set; }
        public decimal Rpm { get; set; }
        public decimal? Constat { get; set; }
        public decimal Efficiency { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateMachineManagerDto
    {
        public string MachineName { get; set; } = string.Empty;
        public decimal Dia { get; set; }
        public decimal Gg { get; set; }
        public int Needle { get; set; }
        public int Feeder { get; set; }
        public decimal Rpm { get; set; }
        public decimal? Constat { get; set; }
        public decimal Efficiency { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class MachineSearchDto
    {
        public string? MachineName { get; set; }
        public decimal? Dia { get; set; }
    }
}