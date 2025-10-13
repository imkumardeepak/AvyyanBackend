using System.ComponentModel.DataAnnotations;

namespace AvyyanBackend.DTOs.StorageCapture
{
    /// <summary>
    /// DTO for StorageCapture data response
    /// </summary>
    public class StorageCaptureResponseDto
    {
        public int Id { get; set; }
        public string LotNo { get; set; } = string.Empty;
        public string FGRollNo { get; set; } = string.Empty;
        public string LocationCode { get; set; } = string.Empty;
        public string Tape { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for creating a new StorageCapture record
    /// </summary>
    public class CreateStorageCaptureRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string LotNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FGRollNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LocationCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Tape { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for updating an existing StorageCapture record
    /// </summary>
    public class UpdateStorageCaptureRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string LotNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FGRollNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LocationCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Tape { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string CustomerName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO for StorageCapture search request
    /// </summary>
    public class StorageCaptureSearchRequestDto
    {
        [MaxLength(100)]
        public string? LotNo { get; set; }

        [MaxLength(100)]
        public string? FGRollNo { get; set; }

        [MaxLength(50)]
        public string? LocationCode { get; set; }

        [MaxLength(100)]
        public string? Tape { get; set; }

        [MaxLength(200)]
        public string? CustomerName { get; set; }

        public bool? IsActive { get; set; }
    }
}