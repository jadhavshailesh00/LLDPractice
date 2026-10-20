using LLD.ParkingLot.Models;

namespace LLD.ParkingLot.Factories;

public static class ParkingFloorFactory
{
    /// <summary>
    /// Creates a standardized floor with formatted Spot IDs (e.g., "F1-COMPACT-01")
    /// </summary>
    public static ParkingFloor CreateFloor(
        int floorNumber,
        int twoWheelerCount,
        int compactCount,
        int largeCount,
        int electricCount)
    {
        var spots = new List<ParkingSpot>();

        AddSpots(spots, floorNumber, SpotType.TwoWheeler, "TW", twoWheelerCount);
        AddSpots(spots, floorNumber, SpotType.Compact, "C", compactCount);
        AddSpots(spots, floorNumber, SpotType.Large, "L", largeCount);
        AddSpots(spots, floorNumber, SpotType.Electric, "EV", electricCount);

        return new ParkingFloor(floorNumber, spots);
    }

    private static void AddSpots(
        List<ParkingSpot> spots,
        int floorNumber,
        SpotType type,
        string prefix,
        int count)
    {
        for (int i = 1; i <= count; i++)
        {
            string spotId = $"F{floorNumber}-{prefix}{i:D2}"; // e.g., F1-C01
            spots.Add(new ParkingSpot(spotId, floorNumber, type));
        }
    }
}