using Fixora.API.Models.Enums;

namespace Fixora.API.Models.InputModels;

public class ElevatorInputModel
{
    public string CustormerFullName { get; set; }
    public string BuildingName { get; set; }
    public string InstalationAddress { get; set; }
    public string CustomerPhoneNumber { get; set; }
    public decimal ElevatorNumberInProject { get; set; }
    public string ElevatorModel { get; set; }
    public string ElevatorSerialNumber { get; set; }
    public string ElevatorProductionCountry { get; set; } // TODO descuss about making enum
    public PostWarrantyType PostWarrantyType { get; set; }
    public DateTime WarrantyDate { get; set; }
}