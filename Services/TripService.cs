using Microsoft.EntityFrameworkCore;
using Tutorial12.Data;
using Tutorial12.DTOs;

namespace Tutorial12.Services;

public class TripService : ITripService
{
    private readonly TripContext _context;

    public TripService(TripContext context)
    {
        _context = context;
    }

    public async Task<object> GetTripsAsync(int page, int pageSize)
    {
        var allTrips = await _context.Trips
            .Include(t => t.ClientTrips).ThenInclude(ct => ct.IdClientNavigation)
            .Include(t => t.IdCountries) // ✅ corrected here
            .OrderByDescending(t => t.DateFrom)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalTrips = await _context.Trips.CountAsync();

        var tripDtos = allTrips.Select(t => new TripDto
        {
            Name = t.Name,
            Description = t.Description,
            DateFrom = t.DateFrom,
            DateTo = t.DateTo,
            MaxPeople = t.MaxPeople,
            Countries = t.IdCountries.Select(c => c.Name).ToList(), // ✅ corrected here
            Clients = t.ClientTrips.Select(c => new ClientDto
            {
                FirstName = c.IdClientNavigation.FirstName,
                LastName = c.IdClientNavigation.LastName
            }).ToList()
        }).ToList();

        return new
        {
            pageNum = page,
            pageSize = pageSize,
            allPages = (int)Math.Ceiling((double)totalTrips / pageSize),
            trips = tripDtos
        };
    }

    public async Task<bool> DeleteClientAsync(int idClient)
    {
        var client = await _context.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(c => c.IdClient == idClient);

        if (client == null) return false;
        if (client.ClientTrips.Any()) return false;

        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<string> AddClientToTripAsync(int idTrip, AddClientToTripRequest request)
    {
        if (await _context.Clients.AnyAsync(c => c.Pesel == request.Pesel))
            return "Client with this PESEL already exists.";

        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.IdTrip == idTrip);
        if (trip == null)
            return "Trip not found.";

        if (trip.DateFrom <= DateTime.Now)
            return "Trip already started.";

        var existingRegistration = await _context.ClientTrips
            .AnyAsync(ct => ct.IdTrip == idTrip && ct.IdClientNavigation.Pesel == request.Pesel);
        if (existingRegistration)
            return "Client already registered for this trip.";

        var client = new Models.Client
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Telephone = request.Telephone,
            Pesel = request.Pesel
        };

        await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();

        var clientTrip = new Models.ClientTrip
        {
            IdClient = client.IdClient,
            IdTrip = idTrip,
            RegisteredAt = DateTime.Now,
            PaymentDate = request.PaymentDate
        };

        await _context.ClientTrips.AddAsync(clientTrip);
        await _context.SaveChangesAsync();

        return "Client registered successfully.";
    }
}