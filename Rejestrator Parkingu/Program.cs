using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

// Klasa reprezentująca pojazd
class Vehicle
{
    public string Brand { get; set; }
    public string LicensePlate { get; set; }
    public DateTime RentalStartDate { get; set; }
    public DateTime RentalEndDate { get; set; }
}

// Klasa reprezentująca miejsce parkingowe
class ParkingSpot
{
    public string SpotNumber { get; set; }
    public bool IsOccupied { get; set; }
    public Vehicle OccupyingVehicle { get; set; }
}

// Klasa zarządzająca bazą danych pojazdów i miejsc parkingowych
class ParkingDatabase
{
    private List<ParkingSpot> parkingSpots;

    public ParkingDatabase()
    {
        parkingSpots = new List<ParkingSpot>();
        // Inicjalizacja miejsc parkingowych
        for (char row = 'A'; row <= 'F'; row++)
        {
            for (int spotNumber = 1; spotNumber <= 10; spotNumber++)
            {
                parkingSpots.Add(new ParkingSpot
                {
                    SpotNumber = $"{spotNumber}{row}",
                    IsOccupied = false,
                    OccupyingVehicle = null
                });
            }
        }

        // Wczytanie danych z pliku
        if (File.Exists("OccupiedSpots.txt"))
        {
            var lines = File.ReadAllLines("OccupiedSpots.txt");
            foreach (var line in lines)
            {
                var data = line.Split(',');
                if (data.Length == 5)
                {
                    string spotNumber = data[0];
                    string brand = data[1];
                    string licensePlate = data[2];
                    DateTime startDate = DateTime.Parse(data[3]);
                    DateTime endDate = DateTime.Parse(data[4]);

                    var vehicle = new Vehicle
                    {
                        Brand = brand,
                        LicensePlate = licensePlate,
                        RentalStartDate = startDate,
                        RentalEndDate = endDate
                    };

                    var spot = parkingSpots.FirstOrDefault(s => s.SpotNumber == spotNumber);
                    if (spot != null)
                    {
                        spot.IsOccupied = true;
                        spot.OccupyingVehicle = vehicle;
                    }
                }
            }
        }
    }

    // Metoda do pobierania miejsca parkingowego na podstawie numeru
    public ParkingSpot GetParkingSpot(string spotNumber)
    {
        return parkingSpots.FirstOrDefault(spot => spot.SpotNumber == spotNumber);
    }

    public void ShowAvailableSpots()
    {
        var availableSpots = parkingSpots.Where(spot => !spot.IsOccupied);
        Console.WriteLine("Dostępne miejsca parkingowe:");
        foreach (var spot in availableSpots)
        {
            Console.WriteLine(spot.SpotNumber);
        }
    }

    public void ShowOccupiedSpots()
    {
        var occupiedSpots = parkingSpots.Where(spot => spot.IsOccupied);
        Console.WriteLine("Zajęte miejsca parkingowe:");
        foreach (var spot in occupiedSpots)
        {
            Console.WriteLine($"Miejsce: {spot.SpotNumber}, Numer rejestracyjny: {spot.OccupyingVehicle.LicensePlate}");
        }
    }

    public void RentSpot(string spotNumber, Vehicle vehicle)
    {
        var spot = parkingSpots.FirstOrDefault(s => s.SpotNumber == spotNumber);
        if (spot != null && !spot.IsOccupied)
        {
            spot.IsOccupied = true;
            spot.OccupyingVehicle = vehicle;
            Console.WriteLine("Miejsce zostało wynajęte.");
            SaveOccupiedSpots(); // Zapisuje zmiany do pliku
        }
        else
        {
            Console.WriteLine("Nie można wynająć tego miejsca.");
        }
    }

    public void GenerateReport()
    {
        int totalSpots = parkingSpots.Count;
        int occupiedSpots = parkingSpots.Count(spot => spot.IsOccupied);
        int availableSpots = totalSpots - occupiedSpots;

        Console.WriteLine($"Liczba miejsc wolnych: {availableSpots}");
        Console.WriteLine($"Liczba miejsc zajętych: {occupiedSpots}");
    }

    // Metoda do zapisywania zajętych miejsc parkingowych do pliku
    public void SaveOccupiedSpots()
    {
        var occupiedSpots = parkingSpots.Where(spot => spot.IsOccupied);
        var lines = occupiedSpots.Select(spot => $"{spot.SpotNumber},{spot.OccupyingVehicle.Brand},{spot.OccupyingVehicle.LicensePlate},{spot.OccupyingVehicle.RentalStartDate},{spot.OccupyingVehicle.RentalEndDate}");
        File.WriteAllLines("OccupiedSpots.txt", lines);
    }
}

// Klasa reprezentująca zwolnienie miejsca parkingowego
class ReleaseSpot
{
    private ParkingDatabase parkingDatabase;

    public ReleaseSpot(ParkingDatabase database)
    {
        parkingDatabase = database;
    }

    public void Release(string spotNumber)
    {
        var spot = parkingDatabase.GetParkingSpot(spotNumber);
        if (spot != null && spot.IsOccupied)
        {
            spot.IsOccupied = false;
            spot.OccupyingVehicle = null;
            Console.WriteLine("Miejsce zostało zwolnione.");
            parkingDatabase.SaveOccupiedSpots(); // Zapisuje zmiany do pliku
        }
        else
        {
            Console.WriteLine("To miejsce nie jest zajęte lub nie istnieje.");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Inicjuj UserManager tylko raz przy rozpoczęciu programu
        UserManager userManager = new UserManager();

        while (true)
        {
            Console.WriteLine("Rejestrator parkingu-logowanie");
            Console.WriteLine("1. Zaloguj się");
            Console.WriteLine("2. Zarejestruj się");
            Console.WriteLine("3. Usuń użytkownika");
            Console.WriteLine("4. Wyjdź");

            int choice;
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("Podaj login.");
                        string username = Console.ReadLine();
                        Console.WriteLine("Podaj hasło.");
                        string password = Console.ReadLine();
                        UserManager.Login(username, password);
                        break;
                    case 2:
                        Console.WriteLine("Podaj login do rejestracji.");
                        string username2 = Console.ReadLine();
                        Console.WriteLine("Podaj hasło.");
                        string password2 = Console.ReadLine();
                        UserManager.CreateUser(username2, password2);
                        break;
                    case 3:
                        Console.WriteLine("Podaj login do usunięcia.");
                        string username3 = Console.ReadLine();
                        UserManager.DeleteUser(username3);
                        break;
                    case 4:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Nieprawidłowa opcja.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Nieprawidłowa opcja.");
            }

            Console.WriteLine();
        }
    }

    public static void Menu()
    {
        ParkingDatabase database = new ParkingDatabase();
        ReleaseSpot releaseSpot = new ReleaseSpot(database);

        while (true)
        {
            Console.WriteLine("Rejestrator parkingu");
            Console.WriteLine("1. Pokaż dostępne miejsca");
            Console.WriteLine("2. Pokaż zajęte miejsca");
            Console.WriteLine("3. Wynajmij miejsce");
            Console.WriteLine("4. Zwolnij miejsce");
            Console.WriteLine("5. Wyłącz aplikację");
            Console.WriteLine("6. Wygeneruj raport");
            Console.Write("Wybierz opcję: ");

            int choice;
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                switch (choice)
                {
                    case 1:
                        database.ShowAvailableSpots();
                        break;
                    case 2:
                        database.ShowOccupiedSpots();
                        break;
                    case 3:
                        Console.Write("Podaj numer miejsca parkingowego: ");
                        string spotNumber = Console.ReadLine();
                        Console.Write("Podaj markę pojazdu: ");
                        string brand = Console.ReadLine();
                        Console.Write("Podaj numer rejestracyjny: ");
                        string licensePlate = Console.ReadLine();
                        Console.Write("Podaj datę rozpoczęcia wynajmu (RRRR-MM-DD): ");
                        if (DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
                        {
                            Console.Write("Podaj datę zakończenia wynajmu (RRRR-MM-DD): ");
                            if (DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
                            {
                                Vehicle vehicle = new Vehicle
                                {
                                    Brand = brand,
                                    LicensePlate = licensePlate,
                                    RentalStartDate = startDate,
                                    RentalEndDate = endDate
                                };
                                database.RentSpot(spotNumber, vehicle);
                            }
                            else
                            {
                                Console.WriteLine("Nieprawidłowy format daty.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowy format daty.");
                        }
                        break;
                    case 4:
                        Console.Write("Podaj numer miejsca parkingowego do zwolnienia: ");
                        string spotToRelease = Console.ReadLine();
                        releaseSpot.Release(spotToRelease);
                        break;
                    case 5:
                        Environment.Exit(0);
                        break;
                    case 6:
                        database.GenerateReport();
                        break;
                    default:
                        Console.WriteLine("Nieprawidłowa opcja.");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Nieprawidłowa opcja.");
            }

            Console.WriteLine();
        }
    }
}
