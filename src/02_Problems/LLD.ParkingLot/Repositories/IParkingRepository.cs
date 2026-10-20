using LLD.ParkingLot.Models;

namespace LLD.ParkingLot.Repositories
{
    public interface IParkingRepository
    {
        IReadOnlyList<ParkingFloor> GetFloors();
        ParkingSpot GetSpotById(string spotId);
        void SaveTicket(Ticket ticket);
        Ticket GetTicket(string ticketId);
    }
}
