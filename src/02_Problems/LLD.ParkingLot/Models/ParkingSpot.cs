namespace LLD.ParkingLot.Models
{
    public sealed class ParkingSpot
    {
        // 0 = Free, 1 = Occupied. Using int with Interlocked gives lock-free thread safety!
        private int _isOccupiedState = 0;

        public string SpotId { get; }
        public int FloorNumber { get; }
        public SpotType SpotType { get; }
        public Vehicle? ParkedVehicle { get; private set; }
        public bool IsOccupied => Volatile.Read(ref _isOccupiedState) == 1;

        public ParkingSpot(string spotId, int floorNumber, SpotType spotType)
        {
            SpotId = spotId;
            FloorNumber = floorNumber;
            SpotType = spotType;
        }

        public bool CanFitVehicle(VehicleType vehicleType) => (vehicleType, SpotType) switch
        {
            (VehicleType.TwoWheeler, SpotType.TwoWheeler) => true,
            (VehicleType.Car, SpotType.Compact or SpotType.Large) => true,
            (VehicleType.ElectricCar, SpotType.Electric or SpotType.Compact) => true,
            (VehicleType.Truck, SpotType.Large) => true,
            _ => false
        };

        /// <summary>
        /// Atomically attempts to claim this spot. If two threads hit this simultaneously,
        /// only one wins the CompareExchange; the other immediately gets false.
        /// </summary>
        public bool TryPark(Vehicle vehicle)
        {
            if (!CanFitVehicle(vehicle.Type))
                return false;

            if (Interlocked.CompareExchange(ref _isOccupiedState, 1, 0) != 0)
                return false; // Another thread just took this spot!

            ParkedVehicle = vehicle;
            return true;
        }

        public void Vacate()
        {
            ParkedVehicle = null;
            Interlocked.Exchange(ref _isOccupiedState, 0);
        }
    }
}
