using System;
using System.Collections.Generic;

namespace VehicleService.Data.Entities;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    public int? ContractId { get; set; }

    public string LicensePlate { get; set; } = null!;

    public string Vin { get; set; } = null!;

    public string? Make { get; set; }

    public string? Model { get; set; }

    public int? Year { get; set; }

    public string? Color { get; set; }

    public decimal? BatteryCapacityKwh { get; set; }

    public string? ChargingPortType { get; set; }

    public DateOnly? PurchaseDate { get; set; }

    public decimal? PurchasePrice { get; set; }

    public int? CoOwnerGroupId { get; set; }

    public int Status { get; set; }
}
