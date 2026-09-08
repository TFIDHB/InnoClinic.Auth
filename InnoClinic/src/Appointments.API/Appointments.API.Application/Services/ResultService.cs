using AutoMapper;
using InnoClinic.Appointments.API.Application.DTOs;
using InnoClinic.Appointments.API.Application.Interfaces;
using InnoClinic.Appointments.API.Domain.Entities;
using InnoClinic.Shared.Exceptions;

namespace InnoClinic.Appointments.API.Application.Services
{
    public class ResultService(
        IAppointmentUnitOfWork unitOfWork,
        IMapper mapper,
        IServicesClient servicesClient,
        IProfilesClient profilesClient,
        IDocumentsClient documentsClient): IResultService
    {
        public async Task<ResultDto> CreateAsync(
            Guid appointmentId,
            CreateResultRequestDto dto,
            Guid doctorId,
            CancellationToken ct = default)
        {
            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(appointmentId, ct)
                ?? throw new NotFoundException(nameof(Appointment));

            var doctorInfo = await profilesClient.GetDoctorInfoAsync(appointment.DoctorId, ct)
                ?? throw new NotFoundException("Doctor");

            if (doctorInfo.AccountId != doctorId)
            {
                throw new ForbiddenException(AppointmentsApiMessages.ForbiddenAccessMessage);
            }

            var existing = await unitOfWork.ResultRepository.GetByAppointmentIdAsync(appointmentId, ct);
            if (existing != null)
            {
                throw new BadRequestException(AppointmentsApiMessages.ResultAlreadyExists);
            }

            var result = mapper.Map<Result>(dto);
            result.AppointmentId = appointmentId;
            result.CreatedAt = DateTime.UtcNow;

            await unitOfWork.ResultRepository.CreateAsync(result, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return await EnrichDtoAsync(appointment, result, canEdit: true, ct);
        }

        public async Task<ResultDto> GetByAppointmentIdAsync(
            Guid appointmentId,
            Guid? doctorId,
            Guid? patientId,
            CancellationToken ct = default)
        {
            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(appointmentId, ct)
                ?? throw new NotFoundException(nameof(Appointment));

            if (patientId.HasValue)
            {
                var patientInfo = await profilesClient.GetPatientInfoAsync(appointment.PatientId, ct)
                    ?? throw new NotFoundException("Patient");

                if (patientInfo.AccountId != patientId.Value)
                {
                    throw new ForbiddenException(AppointmentsApiMessages.ForbiddenAccessMessage);
                }
            }

            var result = await unitOfWork.ResultRepository.GetByAppointmentIdAsync(appointmentId, ct)
                ?? throw new NotFoundException(nameof(Result));

            var canEdit = false;
            if (doctorId.HasValue)
            {
                var doctorInfo = await profilesClient.GetDoctorInfoAsync(appointment.DoctorId, ct);
                canEdit = doctorInfo != null && doctorInfo.AccountId == doctorId.Value;
            }

            return await EnrichDtoAsync(appointment, result, canEdit, ct);
        }

        public async Task<ResultDto> UpdateAsync(
            Guid appointmentId,
            UpdateResultRequestDto dto,
            Guid doctorId,
            CancellationToken ct = default)
        {
            var appointment = await unitOfWork.AppointmentRepository.GetByIdAsync(appointmentId, ct)
                ?? throw new NotFoundException(nameof(Appointment));

            var doctorInfo = await profilesClient.GetDoctorInfoAsync(appointment.DoctorId, ct)
                ?? throw new NotFoundException("Doctor");

            if (doctorInfo.AccountId != doctorId)
            {
                throw new ForbiddenException(AppointmentsApiMessages.ForbiddenAccessMessage);
            }

            var result = await unitOfWork.ResultRepository.GetByAppointmentIdAsync(appointmentId, ct)
                ?? throw new NotFoundException(nameof(Result));

            mapper.Map(dto, result);

            await unitOfWork.ResultRepository.UpdateAsync(result, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return await EnrichDtoAsync(appointment, result, canEdit: true, ct);
        }

        public async Task<byte[]> GetOrGenerateResultFileAsync(
            Guid appointmentId,
            Guid patientId,
            CancellationToken ct = default)
        {
            var dto = await GetByAppointmentIdAsync(appointmentId, doctorId: null, patientId, ct);

            var existingDocument = await documentsClient.GetByResultIdAsync(dto.Id, ct);
            if (existingDocument != null)
            {
                return await documentsClient.DownloadAsync(existingDocument.Url, ct);
            }

            var pdfBytes = ResultPdfGeneratorService.Generate(dto);
            await documentsClient.UploadAsync(dto.Id, pdfBytes, $"appointment-result-{dto.Id}.pdf", ct);

            return pdfBytes;
        }

        private async Task<ResultDto> EnrichDtoAsync(
            Appointment appointment,
            Result result,
            bool canEdit,
            CancellationToken ct)
        {
            var patientInfo = await profilesClient.GetPatientInfoAsync(appointment.PatientId, ct);
            var doctorInfo = await profilesClient.GetDoctorInfoAsync(appointment.DoctorId, ct);
            var serviceName = await servicesClient.GetServiceNameAsync(appointment.ServiceId, ct);
            var specializationName = doctorInfo == null
                ? null
                : await servicesClient.GetSpecializationNameAsync(doctorInfo.SpecializationId, ct);

            var dto = mapper.Map<ResultDto>(result);
            dto.Date = appointment.Date;
            dto.PatientFullName = patientInfo == null
                ? "Unknown patient"
                : $"{patientInfo.LastName} {patientInfo.FirstName} {patientInfo.MiddleName}".Trim();

            dto.PatientDateOfBirth = patientInfo?.DateOfBirth;
            dto.DoctorFullName = doctorInfo == null
                ? "Unknown doctor"
                : $"{doctorInfo.LastName} {doctorInfo.FirstName} {doctorInfo.MiddleName}".Trim();

            dto.DoctorSpecialization = specializationName ?? "Unknown specialization";
            dto.ServiceName = serviceName ?? "Unknown service";
            dto.CanEdit = canEdit;

            return dto;
        }
    }
}
