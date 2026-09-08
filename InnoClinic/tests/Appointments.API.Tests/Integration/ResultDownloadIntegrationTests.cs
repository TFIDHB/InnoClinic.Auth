using InnoClinic.Appointments.API.Application.DTOs;
using InnoClinic.Appointments.API.Application.Interfaces;
using InnoClinic.Appointments.API.Domain.Entities;
using InnoClinic.Appointments.API.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace Appointments.API.Tests.Integration
{
    [Collection("PostgresCollection")]
    public class ResultDownloadIntegrationTests : IAsyncLifetime
    {
        private readonly PostgresContainerFixture _fixture;
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private Mock<IProfilesClient> _profilesClientMock;
        private Mock<IServicesClient> _servicesClientMock;
        private Mock<IDocumentsClient> _documentsClientMock;

        private static void ReplaceWithMock<TInterface>(IServiceCollection services, TInterface instance)
            where TInterface : class
        {
            var descriptor = services.SingleOrDefault(e => e.ServiceType == typeof(TInterface));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddScoped(_ => instance);
        }

        private async Task<(Appointment appointment, Result result)> SeedAppointmentWithResultAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppointmentDbContext>();

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = Guid.NewGuid(),
                SpecializationId = Guid.NewGuid(),
                DoctorId = Guid.NewGuid(),
                ServiceId = Guid.NewGuid(),
                OfficeId = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)),
                Time = new TimeOnly(10, 0),
                Duration = TimeSpan.FromMinutes(30),
                IsApproved = true,
                CreatedAt = DateTime.UtcNow,
            };

            var result = new Result
            {
                Id = Guid.NewGuid(),
                AppointmentId = appointment.Id,
                Complaints = "Headache",
                Conclusion = "Migraine",
                Recommendations = "Rest and hydration",
                CreatedAt = DateTime.UtcNow,
            };

            db.Appointments.Add(appointment);
            db.Results.Add(result);
            await db.SaveChangesAsync();

            return (appointment, result);
        }

        public ResultDownloadIntegrationTests(PostgresContainerFixture fixture)
        {
            _fixture = fixture;
        }

        public Task InitializeAsync()
        {
            _profilesClientMock = new Mock<IProfilesClient>();
            _servicesClientMock = new Mock<IServicesClient>();
            _documentsClientMock = new Mock<IDocumentsClient>();

            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureAppConfiguration((_, config) =>
                    {
                        config.AddInMemoryCollection(new Dictionary<string, string?>
                        {
                            ["JwtSettings:Secret"] = JwtTestTokenFactory.Secret,
                            ["JwtSettings:Issuer"] = JwtTestTokenFactory.Issuer,
                            ["JwtSettings:Audience"] = JwtTestTokenFactory.Audience,
                            ["JwtSettings:ExpirationMinutes"] = "10",
                            ["WorkingHours:Start"] = "08:00",
                            ["WorkingHours:End"] = "20:00",
                        });
                    });

                    builder.ConfigureServices(services =>
                    {
                        var dbDescriptor = services.SingleOrDefault(e => e.ServiceType == typeof(DbContextOptions<AppointmentDbContext>));
                        if (dbDescriptor != null)
                        {
                            services.Remove(dbDescriptor);
                        }

                        services.AddDbContext<AppointmentDbContext>(opt => opt.UseNpgsql(_fixture.PostgresContainer.GetConnectionString()));

                        ReplaceWithMock(services, _profilesClientMock.Object);
                        ReplaceWithMock(services, _servicesClientMock.Object);
                        ReplaceWithMock(services, _documentsClientMock.Object);

                        services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, opt =>
                        {
                            opt.TokenValidationParameters = new TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidateAudience = true,
                                ValidateLifetime = true,
                                ValidateIssuerSigningKey = true,
                                ValidIssuer = JwtTestTokenFactory.Issuer,
                                ValidAudience = JwtTestTokenFactory.Audience,
                                IssuerSigningKey = new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(JwtTestTokenFactory.Secret)),
                            };
                        });
                    });
                });

            _client = _factory.CreateClient();
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            _client?.Dispose();
            await _factory.DisposeAsync();
        }

        [Fact]
        public async Task DownloadResult_WhenAuthorizedPatient_ReturnsPdfAndSavesToDisk()
        {
            var (appointment, result) = await SeedAppointmentWithResultAsync();
            var patientAccountId = Guid.NewGuid();

            _profilesClientMock
                .Setup(x => x.GetPatientInfoAsync(appointment.PatientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PatientInfoDto
                {
                    Id = appointment.PatientId,
                    FirstName = "Jane",
                    LastName = "Doe",
                    AccountId = patientAccountId,
                });

            _profilesClientMock
                .Setup(x => x.GetDoctorInfoAsync(appointment.DoctorId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DoctorInfoDto
                {
                    Id = appointment.DoctorId,
                    FirstName = "John",
                    LastName = "Smith",
                    SpecializationId = appointment.SpecializationId,
                    OfficeId = appointment.OfficeId,
                    AccountId = Guid.NewGuid(),
                });

            _servicesClientMock
                .Setup(x => x.GetServiceNameAsync(appointment.ServiceId, It.IsAny<CancellationToken>()))
                .ReturnsAsync("General checkup");

            _servicesClientMock
                .Setup(x => x.GetSpecializationNameAsync(appointment.SpecializationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync("Therapist");

            _documentsClientMock
                .Setup(x => x.GetByResultIdAsync(result.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((DocumentDto?)null);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", JwtTestTokenFactory.CreatePatientToken(patientAccountId));

            var response = await _client.GetAsync($"/api/v1/appointments/{appointment.Id}/result/download");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/pdf", response.Content.Headers.ContentType?.MediaType);
            var pdfBytes = await response.Content.ReadAsByteArrayAsync();
            Assert.NotEmpty(pdfBytes);

            var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), "DownloadedFiles");
            Directory.CreateDirectory(outputDirectory);

            var filePath = Path.Combine(outputDirectory, $"result_{result.Id}.pdf");
            await File.WriteAllBytesAsync(filePath, pdfBytes);
        }

        [Fact]
        public async Task DownloadResult_WhenPatientDoesNotOwnAppointment_ReturnsForbidden()
        {
            var (appointment, _) = await SeedAppointmentWithResultAsync();

            _profilesClientMock
                .Setup(x => x.GetPatientInfoAsync(appointment.PatientId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PatientInfoDto
                {
                    Id = appointment.PatientId,
                    FirstName = "Jane",
                    LastName = "Doe",
                    AccountId = Guid.NewGuid(),
                });

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", JwtTestTokenFactory.CreatePatientToken(Guid.NewGuid()));

            var response = await _client.GetAsync($"/api/v1/appointments/{appointment.Id}/result/download");

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            _documentsClientMock.Verify(
                x => x.UploadAsync(It.IsAny<Guid>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}