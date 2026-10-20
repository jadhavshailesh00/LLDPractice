using LLD.ParkingLot.Models;
using System.Collections.Concurrent;

namespace LLD.ParkingLot.Repositories
{
    public sealed class InMemoryParkingRepository : IParkingRepository
    {
        private readonly List<ParkingFloor> _floors;
        private readonly Dictionary<string, ParkingSpot> _spotLookup;
        private readonly ConcurrentDictionary<string, Ticket> _tickets = new();

        public InMemoryParkingRepository(IEnumerable<ParkingFloor> floors)
        {
            _floors = floors.ToList();
            _spotLookup = _floors.SelectMany(f => f.Spots).ToDictionary(s => s.SpotId);
        }

        public IReadOnlyList<ParkingFloor> GetFloors() => _floors;
        public ParkingSpot GetSpotById(string spotId) => _spotLookup.GetValueOrDefault(spotId);
        public void SaveTicket(Ticket ticket) => _tickets[ticket.TicketId] = ticket;
        public Ticket GetTicket(string ticketId) => _tickets.GetValueOrDefault(ticketId);
    }
}
