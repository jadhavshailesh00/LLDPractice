using LLD.ParkingLot.Models;

namespace LLD.ParkingLot.Strategies
{
    public interface ISpotAssignmentStrategy
    {
        ParkingSpot? FindAndClaimSpot(IReadOnlyList<ParkingFloor> floors, Vehicle vehicle);
    }
}
