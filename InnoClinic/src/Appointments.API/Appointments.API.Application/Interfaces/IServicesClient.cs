namespace InnoClinic.Appointments.API.Application.Interfaces
{
    public interface IServicesClient
    {
        Task<int> GetTimeSlotSizeAsync(Guid serviceId, CancellationToken ct = default);

        Task<string?> GetServiceNameAsync(Guid serviceId, CancellationToken ct = default);

        Task<string?> GetSpecializationNameAsync(Guid specializationId, CancellationToken ct = default);

        Task<IReadOnlyDictionary<Guid, string>> GetServiceNamesAsync(IEnumerable<Guid> serviceIds, CancellationToken ct = default);
    }
}
