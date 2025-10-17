using AvyyanBackend.Data;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace AvyyanBackend.Repositories
{
    public class DispatchPlanningRepository : IDispatchPlanningRepository
    {
        private readonly ApplicationDbContext _context;

        public DispatchPlanningRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DispatchPlanning> CreateAsync(DispatchPlanning dispatchPlanning)
        {
            _context.DispatchPlannings.Add(dispatchPlanning);
            await _context.SaveChangesAsync();
            return dispatchPlanning;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var dispatchPlanning = await _context.DispatchPlannings.FindAsync(id);
            if (dispatchPlanning == null)
                return false;

            _context.DispatchPlannings.Remove(dispatchPlanning);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DispatchPlanning>> GetAllAsync()
        {
            return await _context.DispatchPlannings.ToListAsync();
        }

        public async Task<DispatchPlanning?> GetByIdAsync(int id)
        {
            return await _context.DispatchPlannings.FindAsync(id);
        }

        public async Task<DispatchPlanning?> GetByLotNoAsync(string lotNo)
        {
            return await _context.DispatchPlannings
                .FirstOrDefaultAsync(dp => dp.LotNo == lotNo);
        }

        public async Task<DispatchPlanning> UpdateAsync(int id, DispatchPlanning dispatchPlanning)
        {
            var existing = await _context.DispatchPlannings.FindAsync(id);
            if (existing == null)
                throw new Exception("DispatchPlanning not found");

            // Update properties
            existing.LotNo = dispatchPlanning.LotNo;
            existing.SalesOrderId = dispatchPlanning.SalesOrderId;
            existing.SalesOrderItemId = dispatchPlanning.SalesOrderItemId;
            existing.CustomerName = dispatchPlanning.CustomerName;
            existing.Tape = dispatchPlanning.Tape;
            existing.TotalRequiredRolls = dispatchPlanning.TotalRequiredRolls;
            existing.TotalReadyRolls = dispatchPlanning.TotalReadyRolls;
            existing.TotalDispatchedRolls = dispatchPlanning.TotalDispatchedRolls;
            existing.IsFullyDispatched = dispatchPlanning.IsFullyDispatched;
            existing.DispatchStartDate = dispatchPlanning.DispatchStartDate;
            existing.DispatchEndDate = dispatchPlanning.DispatchEndDate;
            existing.VehicleNo = dispatchPlanning.VehicleNo;
            existing.DriverName = dispatchPlanning.DriverName;
            existing.License = dispatchPlanning.License;
            existing.MobileNumber = dispatchPlanning.MobileNumber;
            existing.Remarks = dispatchPlanning.Remarks;
            existing.LoadingNo = dispatchPlanning.LoadingNo;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<DispatchedRoll> CreateDispatchedRollAsync(DispatchedRoll dispatchedRoll)
        {
            _context.DispatchedRolls.Add(dispatchedRoll);
            await _context.SaveChangesAsync();
            return dispatchedRoll;
        }

        public async Task<IEnumerable<DispatchedRoll>> GetDispatchedRollsByPlanningIdAsync(int planningId)
        {
            return await _context.DispatchedRolls
                .Where(dr => dr.DispatchPlanningId == planningId)
                .ToListAsync();
        }

        public async Task<string> GenerateLoadingNoAsync()
        {
            // Format: LOAD{YY}{MM}{SERIAL}
            // YY = 2-digit year
            // MM = 2-digit month
            // SERIAL = 4-digit serial number
            
            var now = DateTime.UtcNow;
            var year = now.ToString("yy");
            var month = now.ToString("MM");
            
            // Get the highest serial number for this year and month
            var prefix = $"LOAD{year}{month}";
            var lastRecord = await _context.DispatchPlannings
                .Where(dp => dp.LoadingNo.StartsWith(prefix))
                .OrderByDescending(dp => dp.LoadingNo)
                .FirstOrDefaultAsync();
                
            int nextSerial = 1;
            if (lastRecord != null)
            {
                var lastSerialStr = lastRecord.LoadingNo.Substring(6); // After "LOADYYMM"
                if (int.TryParse(lastSerialStr, out int lastSerial))
                {
                    nextSerial = lastSerial + 1;
                }
            }
            
            return $"{prefix}{nextSerial:D4}"; // Format as 4-digit number with leading zeros
        }
    }
}