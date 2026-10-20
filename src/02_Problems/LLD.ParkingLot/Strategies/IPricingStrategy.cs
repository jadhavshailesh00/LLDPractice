using LLD.ParkingLot.Models;

namespace LLD.ParkingLot.Strategies
{
    public interface IPricingStrategy
    {
        decimal CalculateFee(Ticket ticket, DateTimeOffset exitTime);
    }
}
