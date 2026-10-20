using LLD.ParkingLot.Models;

namespace LLD.ParkingLot.Strategies
{
    /// <summary>
    /// Scans from lowest floor to highest, preferring exact spot match before upgrading (e.g., Compact before Large for a Car).
    /// </summary>
    public sealed class NearestAvailableSpotStrategy : ISpotAssignmentStrategy
    {
        public ParkingSpot? FindAndClaimSpot(IReadOnlyList<ParkingFloor> floors, Vehicle vehicle)
        {
            foreach (var floor in floors.OrderBy(f => f.FloorNumber))
            {
                // Order compatible spots so smaller spots are tried before larger ones
                var candidateSpots = floor.Spots
                    .Where(s => !s.IsOccupied && s.CanFitVehicle(vehicle.Type))
                    .OrderBy(s => s.SpotType);

                foreach (var spot in candidateSpots)
                {
                    // Atomic check-and-claim! If a concurrent thread beat us to it, loop continues to next spot.
                    if (spot.TryPark(vehicle))
                    {
                        return spot;
                    }
                }
            }
            return null;
        }
    }
}
