namespace LLD.ParkingLot.Models
{
    public sealed class ParkingFloor
    {
        public int FloorNumber { get; }
        private readonly List<ParkingSpot> _spots = [];

        public IReadOnlyList<ParkingSpot> Spots => _spots;

        public ParkingFloor(int floorNumber, IEnumerable<ParkingSpot> spots)
        {
            FloorNumber = floorNumber;
            _spots.AddRange(spots);
        }
    }
}
