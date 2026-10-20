namespace LLD.ParkingLot.Exceptions
{

    public sealed class ParkingLotFullException(VehicleType type)
        : Exception($"No available parking spot found for vehicle type: {type}.");

    public sealed class InvalidTicketException(string ticketId)
        : Exception($"Ticket '{ticketId}' is invalid or already processed.");
}
