using System;
using System.Collections.Generic;

namespace BookingService.Data.Entities;

public partial class VehicleCondition
{
    public int VehicleConditionId { get; set; }

    public int CheckInOutId { get; set; }

    public string Name { get; set; } = null!;

    public string? Detail { get; set; }

    public int VehicleId { get; set; }

    public virtual BookingCheckInOut CheckInOut { get; set; } = null!;
}
