using AutoMapper;
using AvyyanBackend.Data;
using AvyyanBackend.DTOs.ProductionConfirmation;
using AvyyanBackend.Models.ProductionConfirmation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RollConfirmationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RollConfirmationController> _logger;
        private readonly IMapper _mapper;

        public RollConfirmationController(ApplicationDbContext context, ILogger<RollConfirmationController> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        // POST api/rollconfirmation
        [HttpPost]
        public async Task<ActionResult<RollConfirmationResponseDto>> CreateRollConfirmation([FromBody] RollConfirmationRequestDto request)
        {
            try
            {
                // Validate request
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if this roll confirmation already exists
                var existingRoll = await _context.RollConfirmations
                    .FirstOrDefaultAsync(r => r.AllotId == request.AllotId && 
                                         r.MachineName == request.MachineName && 
                                         r.RollNo == request.RollNo);
                
                if (existingRoll != null)
                {
                    return Conflict($"Roll confirmation for Allot ID {request.AllotId}, Machine {request.MachineName}, Roll No {request.RollNo} already exists.");
                }

                // Create roll confirmation entity
                var rollConfirmation = new RollConfirmation
                {
                    AllotId = request.AllotId,
                    MachineName = request.MachineName,
                    RollPerKg = request.RollPerKg,
                    GreyGsm = request.GreyGsm,
                    GreyWidth = request.GreyWidth,
                    BlendPercent = request.BlendPercent,
                    Cotton = request.Cotton,
                    Polyester = request.Polyester,
                    Spandex = request.Spandex,
                    RollNo = request.RollNo,
                    CreatedDate = request.CreatedDate
                };

                // Add to database
                _context.RollConfirmations.Add(rollConfirmation);
                await _context.SaveChangesAsync();

                // Create response DTO
                var responseDto = new RollConfirmationResponseDto
                {
                    Id = rollConfirmation.Id,
                    AllotId = rollConfirmation.AllotId,
                    MachineName = rollConfirmation.MachineName,
                    RollPerKg = rollConfirmation.RollPerKg,
                    GreyGsm = rollConfirmation.GreyGsm,
                    GreyWidth = rollConfirmation.GreyWidth,
                    BlendPercent = rollConfirmation.BlendPercent,
                    Cotton = rollConfirmation.Cotton,
                    Polyester = rollConfirmation.Polyester,
                    Spandex = rollConfirmation.Spandex,
                    RollNo = rollConfirmation.RollNo,
                    CreatedDate = rollConfirmation.CreatedDate
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating roll confirmation");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/rollconfirmation/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<RollConfirmationResponseDto>> GetRollConfirmation(int id)
        {
            try
            {
                var rollConfirmation = await _context.RollConfirmations.FindAsync(id);
                
                if (rollConfirmation == null)
                {
                    return NotFound($"Roll confirmation with ID {id} not found.");
                }

                var responseDto = new RollConfirmationResponseDto
                {
                    Id = rollConfirmation.Id,
                    AllotId = rollConfirmation.AllotId,
                    MachineName = rollConfirmation.MachineName,
                    RollPerKg = rollConfirmation.RollPerKg,
                    GreyGsm = rollConfirmation.GreyGsm,
                    GreyWidth = rollConfirmation.GreyWidth,
                    BlendPercent = rollConfirmation.BlendPercent,
                    Cotton = rollConfirmation.Cotton,
                    Polyester = rollConfirmation.Polyester,
                    Spandex = rollConfirmation.Spandex,
                    RollNo = rollConfirmation.RollNo,
                    CreatedDate = rollConfirmation.CreatedDate
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching roll confirmation with ID {Id}", id);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/rollconfirmation/by-allot-id/{allotId}
        [HttpGet("by-allot-id/{allotId}")]
        public async Task<ActionResult<IEnumerable<RollConfirmationResponseDto>>> GetRollConfirmationsByAllotId(string allotId)
        {
            try
            {
                var rollConfirmations = await _context.RollConfirmations
                    .Where(r => r.AllotId == allotId)
                    .ToListAsync();

                var responseDtos = rollConfirmations.Select(roll => new RollConfirmationResponseDto
                {
                    Id = roll.Id,
                    AllotId = roll.AllotId,
                    MachineName = roll.MachineName,
                    RollPerKg = roll.RollPerKg,
                    GreyGsm = roll.GreyGsm,
                    GreyWidth = roll.GreyWidth,
                    BlendPercent = roll.BlendPercent,
                    Cotton = roll.Cotton,
                    Polyester = roll.Polyester,
                    Spandex = roll.Spandex,
                    RollNo = roll.RollNo,
                    CreatedDate = roll.CreatedDate
                }).ToList();

                return Ok(responseDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching roll confirmations for Allot ID {AllotId}", allotId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}