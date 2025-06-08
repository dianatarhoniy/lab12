using Tutorial12.DTOs;

namespace Tutorial12.Services;

public interface ITripService
{
    Task<object> GetTripsAsync(int page, int pageSize);
    Task<bool> DeleteClientAsync(int idClient);
    Task<string> AddClientToTripAsync(int idTrip, AddClientToTripRequest request);
}