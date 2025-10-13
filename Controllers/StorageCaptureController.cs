using AvyyanBackend.Data;
using AvyyanBackend.DTOs.ProductionConfirmation;
using AvyyanBackend.DTOs.StorageCapture;
using AvyyanBackend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class StorageCaptureController : ControllerBase
	{
		private readonly IStorageCaptureService _storageCaptureService;
		private readonly ILogger<StorageCaptureController> _logger;
		private readonly ApplicationDbContext _context;

		public StorageCaptureController(IStorageCaptureService storageCaptureService, ILogger<StorageCaptureController> logger, ApplicationDbContext context)
		{
			_storageCaptureService = storageCaptureService;
			_logger = logger;
			_context = context;
		}

		/// <summary>
		/// Get all storage captures
		/// </summary>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<StorageCaptureResponseDto>>> GetStorageCaptures()
		{
			try
			{
				var storageCaptures = await _storageCaptureService.GetAllStorageCapturesAsync();
				return Ok(storageCaptures);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while getting storage captures");
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		/// <summary>
		/// Get storage capture by ID
		/// </summary>
		[HttpGet("{id}")]
		public async Task<ActionResult<StorageCaptureResponseDto>> GetStorageCapture(int id)
		{
			try
			{
				var storageCapture = await _storageCaptureService.GetStorageCaptureByIdAsync(id);
				if (storageCapture == null)
				{
					return NotFound($"Storage capture with ID {id} not found");
				}
				return Ok(storageCapture);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while getting storage capture {StorageCaptureId}", id);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		/// <summary>
		/// Search storage captures by various criteria
		/// </summary>
		[HttpGet("search")]
		public async Task<ActionResult<IEnumerable<StorageCaptureResponseDto>>> SearchStorageCaptures(
			[FromQuery] string? lotNo,
			[FromQuery] string? fgRollNo,
			[FromQuery] string? locationCode,
			[FromQuery] string? tape,
			[FromQuery] string? customerName,
			[FromQuery] bool? isActive)
		{
			try
			{
				var searchDto = new StorageCaptureSearchRequestDto
				{
					LotNo = lotNo,
					FGRollNo = fgRollNo,
					LocationCode = locationCode,
					Tape = tape,
					CustomerName = customerName,
					IsActive = isActive
				};
				var storageCaptures = await _storageCaptureService.SearchStorageCapturesAsync(searchDto);
				return Ok(storageCaptures);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while searching storage captures");
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		/// <summary>
		/// Create a new storage capture
		/// </summary>
		[HttpPost]
		public async Task<ActionResult<StorageCaptureResponseDto>> CreateStorageCapture(CreateStorageCaptureRequestDto createStorageCaptureDto)
		{
			try
			{
				var storageCapture = await _storageCaptureService.CreateStorageCaptureAsync(createStorageCaptureDto);
				return CreatedAtAction(nameof(GetStorageCapture), new { id = storageCapture.Id }, storageCapture);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while creating storage capture");
				return StatusCode(500, "An error occurred while processing your request");
			}
		}
		/// <summary>
		/// Get roll confirmations by Allot ID
		///</summary>   
		// GET api/storagecapture/by-allot-id/{allotId}
		[HttpGet("by-allot-id/{allotId}")]
		public async Task<ActionResult<IEnumerable<RollConfirmationResponseDto>>> GetRollConfirmationsByAllotId(string allotId)
		{
			try
			{
				var rollConfirmations = await _context.RollConfirmations
					.Where(r => r.AllotId == allotId)
					.ToListAsync();

				if (rollConfirmations == null || rollConfirmations.Count == 0)
				{
					return NotFound($"No roll confirmations found for Allot ID {allotId}.");
				}

				var productionAllotment = await _context.ProductionAllotments
					.Include(pa => pa.MachineAllocations)
					.FirstOrDefaultAsync(pa => pa.AllotmentId == allotId);


				if (productionAllotment == null)
				{
					return NotFound($"Production allotment with Allot ID {allotId} not found.");
				}

				var responseDtos = new
				{
					rollConfirmations = rollConfirmations,
					machineAllocations = productionAllotment.MachineAllocations,
					productionAllotment = productionAllotment
				};

				return Ok(responseDtos);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching roll confirmations for Allot ID {AllotId}", allotId);
				return StatusCode(500, $"Internal server error: {ex.Message}");
			}
		}

		/// <summary>
		/// Update a storage capture
		/// </summary>
		[HttpPut("{id}")]
		public async Task<ActionResult<StorageCaptureResponseDto>> UpdateStorageCapture(int id, UpdateStorageCaptureRequestDto updateStorageCaptureDto)
		{
			try
			{
				var storageCapture = await _storageCaptureService.UpdateStorageCaptureAsync(id, updateStorageCaptureDto);
				if (storageCapture == null)
				{
					return NotFound($"Storage capture with ID {id} not found");
				}
				return Ok(storageCapture);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while updating storage capture {StorageCaptureId}", id);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}

		/// <summary>
		/// Delete a storage capture (soft delete)
		/// </summary>
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteStorageCapture(int id)
		{
			try
			{
				var result = await _storageCaptureService.DeleteStorageCaptureAsync(id);
				if (!result)
				{
					return NotFound($"Storage capture with ID {id} not found");
				}
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while deleting storage capture {StorageCaptureId}", id);
				return StatusCode(500, "An error occurred while processing your request");
			}
		}
	}
}