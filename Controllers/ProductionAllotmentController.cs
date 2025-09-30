﻿﻿﻿﻿﻿﻿﻿using AutoMapper;
using AvyyanBackend.Data;
using AvyyanBackend.DTOs.ProAllotDto;

using AvyyanBackend.Models.ProAllot;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AvyyanBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductionAllotmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductionAllotmentController> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public ProductionAllotmentController(ApplicationDbContext context, ILogger<ProductionAllotmentController> logger, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
        }

        // GET api/productionallotment/next-serial-number
        [HttpGet("next-serial-number")]
        public async Task<ActionResult<string>> GetNextSerialNumber()
        {
            try
            {
                // Get the maximum serial number from existing production allotments and add 1
                var maxSerialNumber = 0;
                var existingAllotments = await _context.ProductionAllotments
                    .Select(pa => pa.SerialNo)
                    .ToListAsync();

                if (existingAllotments.Any())
                {
                    foreach (var serialNo in existingAllotments)
                    {
                        // Parse the serial number (format: "0001", "0002", etc.)
                        if (int.TryParse(serialNo, out int serial))
                        {
                            if (serial > maxSerialNumber)
                                maxSerialNumber = serial;
                        }
                    }
                }

                var nextNumber = maxSerialNumber + 1;

                // Format as 4-digit zero-padded string
                var serialNumber = nextNumber.ToString("D4");

                return Ok(serialNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating next serial number");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/productionallotment/by-allot-id/{allotId}
        [HttpGet("by-allot-id/{allotId}")]
        public async Task<ActionResult<ProductionAllotmentResponseDto>> GetProductionAllotmentByAllotId(string allotId)
        {
            try
            {
                var productionAllotment = await _context.ProductionAllotments
                    .Include(pa => pa.MachineAllocations)
                    .FirstOrDefaultAsync(pa => pa.AllotmentId == allotId);

                if (productionAllotment == null)
                {
                    return NotFound($"Production allotment with ID {allotId} not found.");
                }

                var responseDto = new ProductionAllotmentResponseDto
                {
                    Id = productionAllotment.Id,
                    AllotmentId = productionAllotment.AllotmentId,
                    VoucherNumber = productionAllotment.VoucherNumber,
                    ItemName = productionAllotment.ItemName,
                    SalesOrderId = productionAllotment.SalesOrderId,
                    SalesOrderItemId = productionAllotment.SalesOrderItemId,
                    ActualQuantity = productionAllotment.ActualQuantity,
                    YarnCount = productionAllotment.YarnCount,
                    Diameter = productionAllotment.Diameter,
                    Gauge = productionAllotment.Gauge,
                    FabricType = productionAllotment.FabricType,
                    SlitLine = productionAllotment.SlitLine,
                    StitchLength = productionAllotment.StitchLength,
                    Efficiency = productionAllotment.Efficiency,
                    Composition = productionAllotment.Composition,
                    TotalProductionTime = productionAllotment.MachineAllocations.Sum(ma => ma.EstimatedProductionTime),
                    CreatedDate = productionAllotment.CreatedDate,
                    YarnLotNo = productionAllotment.YarnLotNo,
                    Counter = productionAllotment.Counter,
                    ColourCode = productionAllotment.ColourCode,
                    ReqGreyGsm = productionAllotment.ReqGreyGsm,
                    ReqGreyWidth = productionAllotment.ReqGreyWidth,
                    ReqFinishGsm = productionAllotment.ReqFinishGsm,
                    ReqFinishWidth = productionAllotment.ReqFinishWidth,
                    PartyName = productionAllotment.PartyName,
                    TubeWeight = productionAllotment.TubeWeight,
                    TapeColor = productionAllotment.TapeColor,
                    SerialNo = productionAllotment.SerialNo,
                    MachineAllocations = productionAllotment.MachineAllocations.Select(ma => new MachineAllocationResponseDto
                    {
                        Id = ma.Id,
                        ProductionAllotmentId = ma.ProductionAllotmentId,
                        MachineName = ma.MachineName,
                        MachineId = ma.MachineId,
                        NumberOfNeedles = ma.NumberOfNeedles,
                        Feeders = ma.Feeders,
                        RPM = ma.RPM,
                        RollPerKg = ma.RollPerKg,
                        TotalLoadWeight = ma.TotalLoadWeight,
                        TotalRolls = ma.TotalRolls,
                        RollBreakdown = !string.IsNullOrEmpty(ma.RollBreakdown)
                            ? System.Text.Json.JsonSerializer.Deserialize<DTOs.ProAllotDto.RollBreakdown>(ma.RollBreakdown)
                            : new DTOs.ProAllotDto.RollBreakdown(),
                        EstimatedProductionTime = ma.EstimatedProductionTime
                    }).ToList()
                };

                return Ok(responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production allotment by AllotmentId: {AllotmentId}", allotId);
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/productionallotment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductionAllotmentResponseDto>>> GetAllProductionAllotments()
        {
            try
            {
                var productionAllotments = await _context.ProductionAllotments
                    .Include(pa => pa.MachineAllocations)
                    .ToListAsync();

                var responseDtos = productionAllotments.Select(pa => new ProductionAllotmentResponseDto
                {
                    Id = pa.Id,
                    AllotmentId = pa.AllotmentId,
                    VoucherNumber = pa.VoucherNumber,
                    ItemName = pa.ItemName,
                    SalesOrderId = pa.SalesOrderId,
                    SalesOrderItemId = pa.SalesOrderItemId,
                    ActualQuantity = pa.ActualQuantity,
                    YarnCount = pa.YarnCount,
                    Diameter = pa.Diameter,
                    Gauge = pa.Gauge,
                    FabricType = pa.FabricType,
                    SlitLine = pa.SlitLine,
                    StitchLength = pa.StitchLength,
                    Efficiency = pa.Efficiency,
                    Composition = pa.Composition,
                    TotalProductionTime = pa.TotalProductionTime,
                    CreatedDate = pa.CreatedDate,
                    YarnLotNo = pa.YarnLotNo,
                    Counter = pa.Counter,
                    ColourCode = pa.ColourCode,
                    ReqGreyGsm = pa.ReqGreyGsm,
                    ReqGreyWidth = pa.ReqGreyWidth,
                    ReqFinishGsm = pa.ReqFinishGsm,
                    ReqFinishWidth = pa.ReqFinishWidth,
                    PartyName = pa.PartyName,
                    TubeWeight = pa.TubeWeight,
                    TapeColor = pa.TapeColor,
                    SerialNo = pa.SerialNo,
                    MachineAllocations = pa.MachineAllocations.Select(ma => new MachineAllocationResponseDto
                    {
                        Id = ma.Id,
                        ProductionAllotmentId = ma.ProductionAllotmentId,
                        MachineName = ma.MachineName,
                        MachineId = ma.MachineId,
                        NumberOfNeedles = ma.NumberOfNeedles,
                        Feeders = ma.Feeders,
                        RPM = ma.RPM,
                        RollPerKg = ma.RollPerKg,
                        TotalLoadWeight = ma.TotalLoadWeight,
                        TotalRolls = ma.TotalRolls,
                        RollBreakdown = !string.IsNullOrEmpty(ma.RollBreakdown)
                            ? System.Text.Json.JsonSerializer.Deserialize<DTOs.ProAllotDto.RollBreakdown>(ma.RollBreakdown)
                            : new DTOs.ProAllotDto.RollBreakdown(),
                        EstimatedProductionTime = ma.EstimatedProductionTime
                    }).ToList()
                }).ToList();

                return Ok(responseDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching production allotments");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        ////  Sticker Printing Logic
        [HttpPost("stkprint/{id}")]
        public IActionResult stkprint(int id) // MachineAllocation id
        {
            try
            {
                // Get the specific machine allocation with its parent production allotment
                var machineAllocation = _context.MachineAllocations
                    .Include(ma => ma.ProductionAllotment)
                    .FirstOrDefault(ma => ma.Id == id);

                if (machineAllocation == null)
                {
                    return NotFound($"Machine allocation with ID {id} not found.");
                }

                string filepath = Path.Combine("wwwroot", "Sticker", "MLRoll.prn");
                string printerName = _configuration["Printers:Printer_IP"];

                if (!System.IO.File.Exists(filepath))
                {
                    return StatusCode(500, "PRN template file not found.");
                }

                // Read the PRN file content
                string fileContent = System.IO.File.ReadAllText(filepath);

                // Parse roll breakdown to determine roll numbers
                var rollBreakdown = JsonConvert.DeserializeObject<DTOs.ProAllotDto.RollBreakdown>(machineAllocation.RollBreakdown);
                int totalRolls = (int)machineAllocation.TotalRolls;

                // Generate QR codes for each roll
                for (int rollNumber = 1; rollNumber <= totalRolls; rollNumber++)
                {
                    string currentFileContent = fileContent;

                    // Replace placeholders with actual values
                    currentFileContent = currentFileContent
                        .Replace("<MCCODE>", machineAllocation.MachineName.Trim())
                        .Replace("<LCODE>", machineAllocation.ProductionAllotment.AllotmentId.Trim())
                        .Replace("<ROLLNO>", rollNumber.ToString())
                        .Replace("<YCOUNT>", machineAllocation.ProductionAllotment.YarnCount?.Trim() ?? "")
                        .Replace("<DIAGG>", $"{machineAllocation.ProductionAllotment.Diameter} X {machineAllocation.ProductionAllotment.Gauge}")
                        .Replace("<STICHLEN>", machineAllocation.ProductionAllotment.StitchLength.ToString("F2"))
                        .Replace("<FEBTYP>", machineAllocation.ProductionAllotment.FabricType?.Trim() ?? "")
                        .Replace("<COMP>", machineAllocation.ProductionAllotment.Composition?.Trim() ?? "");

                    // Send to printer
                    PrintToNetworkPrinter(printerName, currentFileContent);
                }

                return Ok(new { message = $"{totalRolls} QR codes printed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error printing QR codes: {ex.Message}");
            }
        }

        private void PrintToNetworkPrinter(string printerIp, string content)
        {
            try
            {
                var printerAddress = IPAddress.Parse(printerIp);
                var printerPort = 9100;

                // Check if printer is reachable
                Ping ping = new Ping();
                PingReply reply = ping.Send(printerAddress, 1000);

                if (reply.Status != IPStatus.Success)
                {
                    throw new Exception($"Printer at {printerIp} is not reachable.");
                }

                // Connect and send print data
                using (var client = new TcpClient())
                {
                    client.Connect(printerAddress, printerPort);
                    byte[] prnData = Encoding.ASCII.GetBytes(content);

                    using (var stream = client.GetStream())
                    {
                        stream.Write(prnData, 0, prnData.Length);
                        stream.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to print: {ex.Message}");
            }
        }


        // POST api/productionallotment
        [HttpPost]
        public async Task<IActionResult> CreateProductionAllotment([FromBody] CreateProductionAllotmentRequest request)
        {
            try
            {
                // Validate request
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Generate the next serial number
                var nextSerialNumber = await GenerateNextSerialNumber();
                
                // Check if allotment ID already exists (optional duplicate check)
                if (await _context.ProductionAllotments.AnyAsync(pa => pa.AllotmentId == request.AllotmentId))
                {
                    return Conflict($"Allotment ID {request.AllotmentId} already exists.");
                }

                // Create production allotment using the allotmentId from the request
                var productionAllotment = new ProductionAllotment
                {
                    AllotmentId = request.AllotmentId, // Use the ID from frontend
                    VoucherNumber = request.VoucherNumber,
                    ItemName = request.ItemName,
                    SalesOrderId = request.SalesOrderId,
                    SalesOrderItemId = request.SalesOrderItemId,
                    ActualQuantity = request.ActualQuantity,
                    YarnCount = request.YarnCount,
                    Diameter = request.Diameter,
                    Gauge = request.Gauge,
                    FabricType = request.FabricType,
                    SlitLine = request.SlitLine,
                    StitchLength = request.StitchLength,
                    Efficiency = request.Efficiency,
                    Composition = request.Composition,
                    YarnLotNo = request.YarnLotNo,
                    Counter = request.Counter,
                    ColourCode = request.ColourCode,
                    ReqGreyGsm = request.ReqGreyGsm,
                    ReqGreyWidth = request.ReqGreyWidth,
                    ReqFinishGsm = request.ReqFinishGsm,
                    ReqFinishWidth = request.ReqFinishWidth,
                    PartyName = request.PartyName,
                    TubeWeight = request.TubeWeight,
                    TapeColor = request.TapeColor,
                    SerialNo = nextSerialNumber, // Assign the generated serial number
                    TotalProductionTime = request.MachineAllocations.Max(ma => ma.EstimatedProductionTime),
                    MachineAllocations = request.MachineAllocations.Select(ma => new MachineAllocation
                    {
                        MachineName = ma.MachineName,
                        MachineId = ma.MachineId,
                        NumberOfNeedles = ma.NumberOfNeedles,
                        Feeders = ma.Feeders,
                        RPM = ma.RPM,
                        RollPerKg = ma.RollPerKg,
                        TotalLoadWeight = ma.TotalLoadWeight,
                        TotalRolls = ma.TotalRolls,
                        RollBreakdown = System.Text.Json.JsonSerializer.Serialize(ma.RollBreakdown),
                        EstimatedProductionTime = ma.EstimatedProductionTime
                    }).ToList()
                };

                // Save to database
                _context.ProductionAllotments.Add(productionAllotment);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Success = true,
                    AllotmentId = request.AllotmentId,
                    ProductionAllotmentId = productionAllotment.Id,
                    SerialNo = nextSerialNumber
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating production allotment");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Helper method to generate the next serial number
        private async Task<string> GenerateNextSerialNumber()
        {
            // Get the maximum serial number from existing production allotments and add 1
            var maxSerialNumber = 0;
            var existingAllotments = await _context.ProductionAllotments
                .Select(pa => pa.SerialNo)
                .ToListAsync();

            if (existingAllotments.Any())
            {
                foreach (var serialNo in existingAllotments)
                {
                    // Parse the serial number (format: "0001", "0002", etc.)
                    if (int.TryParse(serialNo, out int serial))
                    {
                        if (serial > maxSerialNumber)
                            maxSerialNumber = serial;
                    }
                }
            }

            var nextNumber = maxSerialNumber + 1;

            // Format as 4-digit zero-padded string
            return nextNumber.ToString("D4");
        }
    }
}