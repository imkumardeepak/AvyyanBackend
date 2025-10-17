namespace AvyyanBackend.DTOs.DispatchPlanning
{
    public class UpdateDispatchPlanningDto
    {
        public string LotNo { get; set; } = string.Empty;
        public int SalesOrderId { get; set; }
        public int SalesOrderItemId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Tape { get; set; } = string.Empty;
        public decimal TotalRequiredRolls { get; set; }
        public decimal TotalReadyRolls { get; set; }
        public decimal TotalDispatchedRolls { get; set; }
        public bool IsFullyDispatched { get; set; }
        public DateTime? DispatchStartDate { get; set; }
        public DateTime? DispatchEndDate { get; set; }
        public string VehicleNo { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string License { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string LoadingNo { get; set; } = string.Empty;
    }
}