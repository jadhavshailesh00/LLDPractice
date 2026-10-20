using LLD.ParkingLot.Exceptions;
using LLD.ParkingLot.Factories;
using LLD.ParkingLot.Repositories;
using LLD.ParkingLot.Services;
using LLD.ParkingLot.Strategies;

// 1. Initialize Floors using ParkingFloorFactory
var floor1 = ParkingFloorFactory.CreateFloor(floorNumber: 1, twoWheelerCount: 2, compactCount: 2, largeCount: 1, electricCount: 1);
var floor2 = ParkingFloorFactory.CreateFloor(floorNumber: 2, twoWheelerCount: 1, compactCount: 2, largeCount: 1, electricCount: 0);

// 2. Wire up Dependencies (Repository + Strategies + Service)
IParkingRepository repository = new InMemoryParkingRepository([floor1, floor2]);
ISpotAssignmentStrategy spotStrategy = new NearestAvailableSpotStrategy();
IPricingStrategy pricingStrategy = new HourlyPricingStrategy();

var parkingService = new ParkingLotService(repository, spotStrategy, pricingStrategy);

// 3. Start Interactive CLI Loop
PrintBanner();
PrintLotStatus(repository);

while (true)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("\nparking-cli> ");
    Console.ResetColor();

    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrWhiteSpace(input)) continue;

    var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var command = parts[0].ToUpperInvariant();

    try
    {
        switch (command)
        {
            case "PARK":
                if (parts.Length < 3)
                {
                    Console.WriteLine("Usage: PARK <TwoWheeler|Car|Truck|ElectricCar> <LicensePlate>");
                    break;
                }
                // Uses VehicleFactory to parse raw string input into a domain model
                var vehicle = VehicleFactory.CreateFromCliInput(parts[1], parts[2]);
                var ticket = parkingService.ParkVehicle(vehicle);
                PrintSuccess($"[PARKED] Ticket ID: {ticket.TicketId} | Spot: {ticket.SpotId} (Floor {ticket.FloorNumber}) | Plate: {ticket.LicensePlate}");
                break;

            case "UNPARK":
                if (parts.Length < 2)
                {
                    Console.WriteLine("Usage: UNPARK <TicketId>");
                    break;
                }
                var paidTicket = parkingService.UnparkVehicle(parts[1].ToUpperInvariant());
                PrintSuccess($"[UNPARKED] Ticket ID: {paidTicket.TicketId} | Spot Vacated: {paidTicket.SpotId} | Fee Paid: Rs. {paidTicket.AmountCharged}");
                break;

            case "STATUS":
                PrintLotStatus(repository);
                break;

            case "DEMO":
                RunAutomatedScenario(parkingService, repository);
                break;

            case "HELP":
                PrintBanner();
                break;

            case "EXIT":
                Console.WriteLine("Shutting down Parking Lot CLI...");
                return;

            default:
                PrintError($"Unknown command '{command}'. Type HELP for available commands.");
                break;
        }
    }
    catch (ParkingLotFullException ex)
    {
        PrintError($"[LOT FULL] {ex.Message}");
    }
    catch (InvalidTicketException ex)
    {
        PrintError($"[INVALID TICKET] {ex.Message}");
    }
    catch (ArgumentException ex)
    {
        PrintError($"[BAD INPUT] {ex.Message}");
    }
    catch (Exception ex)
    {
        PrintError($"[ERROR] {ex.Message}");
    }
}

// --- HELPER METHODS FOR VISUALIZATION & DEMO ---

static void RunAutomatedScenario(ParkingLotService service,IParkingRepository repo)

{
    Console.WriteLine("\n--- Running Automated Interview Demo Scenario ---");

    // 1. Park 2 Trucks (Only 2 Large spots exist across Floor 1 & Floor 2)
    var t1 = service.ParkVehicle(VehicleFactory.CreateFromCliInput("Truck", "MH-12-TR-1001"));
    PrintSuccess($"1. Parked Truck 1 -> Spot {t1.SpotId} (Ticket: {t1.TicketId})");

    var t2 = service.ParkVehicle(VehicleFactory.CreateFromCliInput("Truck", "MH-12-TR-1002"));
    PrintSuccess($"2. Parked Truck 2 -> Spot {t2.SpotId} (Ticket: {t2.TicketId})");

    // 2. Try parking a 3rd Truck (Should trigger ParkingLotFullException)
    try
    {
        Console.WriteLine("3. Attempting to park 3rd Truck when all Large spots are full...");
        service.ParkVehicle(VehicleFactory.CreateFromCliInput("Truck", "MH-12-TR-9999"));
    }
    catch (ParkingLotFullException ex)
    {
        PrintError($"   Expected Exception Caught: {ex.Message}");
    }

    // 3. Park a Car and an EV
    var c1 = service.ParkVehicle(VehicleFactory.CreateFromCliInput("Car", "MH-12-AB-3000"));
    PrintSuccess($"4. Parked Car -> Spot {c1.SpotId} (Ticket: {c1.TicketId})");

    PrintLotStatus(repo);

    // 4. Unpark Truck 1 and show spot freed
    var receipt = service.UnparkVehicle(t1.TicketId);
    PrintSuccess($"5. Unparked Truck 1 (Ticket: {receipt.TicketId}) | Billed: Rs. {receipt.AmountCharged}");

    PrintLotStatus(repo);
}

static void PrintLotStatus(IParkingRepository repo)
{
    Console.WriteLine("\n================ PARKING LOT LIVE MAP ================");
    foreach (var floor in repo.GetFloors())
    {
        Console.Write($"Floor {floor.FloorNumber}: ");
        foreach (var spot in floor.Spots)
        {
            if (spot.IsOccupied)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"[{spot.SpotId}: {spot.ParkedVehicle?.LicensePlate}] ");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"[{spot.SpotId}: FREE] ");
            }
        }
        Console.ResetColor();
        Console.WriteLine();
    }
    Console.WriteLine("======================================================");
}

static void PrintBanner()
{
    Console.WriteLine("======================================================");
    Console.WriteLine("         PARKING LOT LLD - MACHINE CODING CLI         ");
    Console.WriteLine("======================================================");
    Console.WriteLine("Commands:");
    Console.WriteLine("  DEMO                                      -> Run automated end-to-end showcase");
    Console.WriteLine("  PARK <TwoWheeler|Car|Truck|ElectricCar> <Plate> -> Park a vehicle (e.g., PARK Car MH-12-1234)");
    Console.WriteLine("  UNPARK <TicketId>                         -> Exit and calculate fee");
    Console.WriteLine("  STATUS                                    -> Show live floor-by-floor spot map");
    Console.WriteLine("  EXIT                                      -> Quit application");
}

static void PrintSuccess(string message)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(message);
    Console.ResetColor();
}

static void PrintError(string message)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine(message);
    Console.ResetColor();
}