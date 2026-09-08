using InnoClinic.Appointments.API.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InnoClinic.Appointments.API.Application.Services
{
    public static class ResultPdfGeneratorService
    {
        public static byte[] Generate(ResultDto result)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontFamily("Times New Roman").FontSize(14));
                    page.Header().Text("Appointment Result").FontSize(18).Bold();

                    page.Content().Column(column =>
                    {
                        column.Spacing(8);
                        column.Item().Text($"Date: {result.Date:yyyy-MM-dd}");
                        column.Item().Text($"Patient: {result.PatientFullName}");
                        column.Item().Text($"Patient date of birth: {result.PatientDateOfBirth:yyyy-MM-dd}");
                        column.Item().Text($"Doctor: {result.DoctorFullName}");
                        column.Item().Text($"Specialization: {result.DoctorSpecialization}");
                        column.Item().Text($"Service: {result.ServiceName}");

                        column.Item().PaddingTop(10).Text("Complaints").Bold();
                        column.Item().Text(result.Complaints);

                        column.Item().PaddingTop(10).Text("Conclusion").Bold();
                        column.Item().Text(result.Conclusion);

                        column.Item().PaddingTop(10).Text("Recommendations").Bold();
                        column.Item().Text(result.Recommendations);
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
