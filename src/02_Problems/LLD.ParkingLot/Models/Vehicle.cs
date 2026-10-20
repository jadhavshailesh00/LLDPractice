namespace LLD.ParkingLot.Models
{

    public abstract record Vehicle(string LicensePlate, VehicleType Type);
    public sealed record Bike(string LicensePlate) : Vehicle(LicensePlate, VehicleType.TwoWheeler);
    public sealed record Car(string LicensePlate) : Vehicle(LicensePlate, VehicleType.Car);
    public sealed record Truck(string LicensePlate) : Vehicle(LicensePlate, VehicleType.Truck);
    public sealed record ElectricCar(string LicensePlate) : Vehicle(LicensePlate, VehicleType.ElectricCar);
}
