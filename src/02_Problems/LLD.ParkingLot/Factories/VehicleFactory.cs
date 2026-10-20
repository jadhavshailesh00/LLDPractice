using LLD.ParkingLot.Models;

namespace LLD.ParkingLot.Factories;

public static class VehicleFactory
{
    public static Vehicle CreateVehicle(VehicleType type, string licensePlate)
    {
        if (string.IsNullOrWhiteSpace(licensePlate))
            throw new ArgumentException("License plate cannot be empty.", nameof(licensePlate));

        return type switch
        {
            VehicleType.TwoWheeler => new Bike(licensePlate),
            VehicleType.Car => new Car(licensePlate),
            VehicleType.Truck => new Truck(licensePlate),
            VehicleType.ElectricCar => new ElectricCar(licensePlate),
            _ => throw new ArgumentOutOfRangeException(nameof(type), $"Unsupported vehicle type: {type}")
        };
    }

    // Helpful for CLI-based Machine Coding rounds (e.g., input: "CAR", "MH-12-1234")
    public static Vehicle CreateFromCliInput(string rawType, string licensePlate)
    {
        if (!Enum.TryParse<VehicleType>(rawType, ignoreCase: true, out var parsedType))
            throw new ArgumentException($"Invalid vehicle type string: '{rawType}'");

        return CreateVehicle(parsedType, licensePlate);
    }
}