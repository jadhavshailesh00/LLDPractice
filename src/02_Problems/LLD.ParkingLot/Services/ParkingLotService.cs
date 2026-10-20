using LLD.ParkingLot.Exceptions;
using LLD.ParkingLot.Models;
using LLD.ParkingLot.Repositories;
using LLD.ParkingLot.Strategies;

namespace LLD.ParkingLot.Services;

public sealed class ParkingLotService
{
    private readonly IParkingRepository _repository;
    private readonly ISpotAssignmentStrategy _spotStrategy;
    private readonly IPricingStrategy _pricingStrategy;
    private readonly TimeProvider _timeProvider;

    public ParkingLotService(
        IParkingRepository repository,
        ISpotAssignmentStrategy spotStrategy,
        IPricingStrategy pricingStrategy,
        TimeProvider? timeProvider = null)
    {
        _repository = repository;
        _spotStrategy = spotStrategy;
        _pricingStrategy = pricingStrategy;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    public Ticket ParkVehicle(Vehicle vehicle)
    {
        var claimedSpot = _spotStrategy.FindAndClaimSpot(_repository.GetFloors(), vehicle)
            ?? throw new ParkingLotFullException(vehicle.Type);

        var ticket = new Ticket(vehicle.LicensePlate, claimedSpot, _timeProvider.GetUtcNow());
        _repository.SaveTicket(ticket);
        return ticket;
    }

    public Ticket UnparkVehicle(string ticketId)
    {
        var ticket = _repository.GetTicket(ticketId)
            ?? throw new InvalidTicketException(ticketId);

        lock (ticket) // Prevent double-payment race condition on the exact same ticket
        {
            if (ticket.Status != TicketStatus.Active)
                throw new InvalidTicketException(ticketId);

            var spot = _repository.GetSpotById(ticket.SpotId)
                ?? throw new InvalidOperationException($"Spot {ticket.SpotId} missing.");

            var exitTime = _timeProvider.GetUtcNow();
            var fee = _pricingStrategy.CalculateFee(ticket, exitTime);

            ticket.MarkPaid(fee, exitTime);
            spot.Vacate();
            return ticket;
        }
    }
}