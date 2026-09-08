using InnoClinic.Appointments.API.Application.Interfaces;
using InnoClinic.Appointments.API.Domain.Entities;
using InnoClinic.Appointments.API.Infrastructure.Persistence;
using InnoClinic.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace InnoClinic.Appointments.API.Infrastructure.Persistence.Repositories
{
    public class AppointmentRepository(AppointmentDbContext context) : BaseRepository<Appointment, Guid>(context), IAppointmentRepository
    {
        public async Task<IEnumerable<Appointment>> GetByDateAndDoctorAsync(
            DateOnly date,
            Guid? doctorId,
            CancellationToken ct = default)
        {
            return await context.Appointments
                .Where(a => a.Date == date && (doctorId == null || a.DoctorId == doctorId))
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Appointment>> GetByDateRangeAndDoctorAsync(
            DateOnly startDate,
            DateOnly endDate,
            Guid? doctorId,
            CancellationToken ct = default)
        {
            return await context.Appointments
                .Where(a => a.Date >= startDate && a.Date <= endDate && (doctorId == null || a.DoctorId == doctorId))
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Appointment>> GetByPatientAsync(Guid patientId, CancellationToken ct = default)
        {
            return await context.Appointments
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.Date)
                .ThenBy(a => a.Time)
                .ToListAsync(ct);
        }

        public async Task<IEnumerable<Appointment>> GetFilteredAsync(
            DateOnly? date,
            Guid? officeId,
            bool? isApproved,
            CancellationToken ct = default)
        {
            var query = context.Appointments.AsQueryable();

            if (date != null)
            {
                query = query.Where(a => a.Date == date.Value);
            }

            if (officeId != null)
            {
                query = query.Where(a => a.OfficeId == officeId.Value);
            }

            if (isApproved != null)
            {
                query = query.Where(a => a.IsApproved == isApproved.Value);
            }

            return await query.ToListAsync(ct);
        }
    }
}
