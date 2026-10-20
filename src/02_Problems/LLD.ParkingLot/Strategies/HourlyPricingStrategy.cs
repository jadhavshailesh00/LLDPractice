using LLD.ParkingLot.Models;

namespace LLD.ParkingLot.Strategies
{
    public sealed class HourlyPricingStrategy : IPricingStrategy
    {
        private readonly IReadOnlyDictionary<SpotType, decimal> _hourlyRates = new Dictionary<SpotType, decimal>
        {
            [SpotType.TwoWheeler] = 20m,
            [SpotType.Compact] = 50m,
            [SpotType.Electric] = 60m,
            [SpotType.Large] = 80m
        };

        public decimal CalculateFee(Ticket ticket, DateTimeOffset exitTime)
        {
            var duration = exitTime - ticket.EntryTime;
            // Minimum 1 hour charge, rounded up to the next ceiling hour
            var billableHours = Math.Max(1, (int)Math.Ceiling(duration.TotalHours));
            return billableHours * _hourlyRates[ticket.SpotType];
        }
    }
}
