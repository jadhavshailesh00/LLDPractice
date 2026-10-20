namespace LLD.ParkingLot.Models
{
    public sealed class Ticket
    {
        public string TicketId { get; } = Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
        public string LicensePlate { get; }
        public string SpotId { get; }
        public int FloorNumber { get; }
        public SpotType SpotType { get; }
        public DateTimeOffset EntryTime { get; }
        public DateTimeOffset? ExitTime { get; private set; }
        public decimal? AmountCharged { get; private set; }
        public TicketStatus Status { get; private set; } = TicketStatus.Active;

        public Ticket(string licensePlate, ParkingSpot spot, DateTimeOffset entryTime)
        {
            LicensePlate = licensePlate;
            SpotId = spot.SpotId;
            FloorNumber = spot.FloorNumber;
            SpotType = spot.SpotType;
            EntryTime = entryTime;
        }

        public void MarkPaid(decimal amount, DateTimeOffset exitTime)
        {
            AmountCharged = amount;
            ExitTime = exitTime;
            Status = TicketStatus.Paid;
        }
    }
}
