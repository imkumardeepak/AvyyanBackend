﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿﻿using AutoMapper;
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
        // Get the current financial year (last two digits)
        var currentYear = DateTime.Now.Year % 100;
        if (DateTime.Now.Month < 4) // If before April, use previous financial year
        {
            currentYear = (DateTime.Now.Year - 1) % 100;
        }

        // Format the current financial year as a string
        var currentFinancialYear = currentYear.ToString("D2");

        // Use a more precise query to get allotments from the current financial year
        var startDate = new DateTime(DateTime.Now.Month < 4 ? DateTime.Now.Year - 1 : DateTime.Now.Year, 4, 1);
        var endDate = startDate.AddYears(1).AddDays(-1);

        // Get all existing production allotments for the current financial year
        var existingAllotments = await _context.ProductionAllotments
            .Where(pa => pa.CreatedDate >= startDate && pa.CreatedDate <= endDate)
            .Select(pa => pa.AllotmentId)
            .ToListAsync();

        var maxSerialNumber = 0;
        
        foreach (var allotmentId in existingAllotments)
        {
            // Expected format: X{2}X{1}X{1}XX{2}C{2}{2}-{4}[SERIAL]{1}
            // Example: ASJL-130C3028-250001N
            var parts = allotmentId.Split('-');
            if (parts.Length >= 3)
            {
                var lastPart = parts[2]; // Get the last part (e.g., "250001N")
                
                // Check if it's at least 7 characters
                if (lastPart.Length >= 7)
                {
                    // Extract the financial year part (first 2 characters)
                    var financialYearPart = lastPart.Substring(0, 2);
                    
                    // Check if it matches current financial year
                    if (financialYearPart == currentFinancialYear)
                    {
                        // Extract the serial number part (positions 2-5: 4 digits)
                        var serialPart = lastPart.Substring(2, 4);
                        if (int.TryParse(serialPart, out int serial))
                        {
                            if (serial > maxSerialNumber)
                                maxSerialNumber = serial;
                        }
                    }
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
                    ProductionAllotmentId = productionAllotment.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating production allotment");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET api/productionallotment/generate-allotment-id
[HttpGet("generate-allotment-id")]
public async Task<ActionResult<string>> GenerateAllotmentId([FromQuery] string firstChar, [FromQuery] string fabricTypeCode, 
    [FromQuery] string fourthChar, [FromQuery] string fifthChar, [FromQuery] string yarnCount, 
    [FromQuery] string eighthChar, [FromQuery] string machineDiameter, [FromQuery] string machineGauge,
    [FromQuery] string financialYear, [FromQuery] string twentyFirstChar)
{
    try
    {
        // Get next serial number
        var serialNumberResponse = await GetNextSerialNumber();
        if (serialNumberResponse.Result is OkObjectResult okResult)
        {
            var serialNumber = okResult.Value.ToString();
            
            // Format with hyphens: ASJL-130C3028-25000001N
            // Hyphens after 4th and 12th characters
            var part1 = $"{firstChar}{fabricTypeCode}{fourthChar}"; // First 4 characters
            var part2 = $"{fifthChar}{yarnCount}{eighthChar}{machineDiameter}{machineGauge}"; // Next 8 characters (5-12)
            var part3 = $"{financialYear}{serialNumber}{twentyFirstChar}"; // Remaining characters (13-21)

            return Ok($"{part1}-{part2}-{part3}");
        }
        else
        {
            return serialNumberResponse.Result;
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error generating allotment ID");
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
}

    }
}             