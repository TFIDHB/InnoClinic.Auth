using InnoClinic.Appointments.API.Application.DTOs;
using InnoClinic.Appointments.API.Application.Interfaces;
using InnoClinic.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace InnoClinic.Appointments.API.Infrastructure.Clients
{
    public class DocumentsClient(HttpClient httpClient) : IDocumentsClient
    {
        public async Task<byte[]> DownloadAsync(string url, CancellationToken ct = default)
        {
            return await httpClient.GetByteArrayAsync(url, ct);
        }

        public async Task<DocumentDto?> GetByResultIdAsync(Guid resultId, CancellationToken ct = default)
        {
            var response = await httpClient.GetAsync($"/api/v1/documents/by-result/{resultId}", ct);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<DocumentDto>(ct);
        }

        public async Task<DocumentDto> UploadAsync(
            Guid resultId,
            byte[] fileBytes,
            string fileName,
            CancellationToken ct = default)
        {
            using var content = new MultipartFormDataContent();
            using var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

            content.Add(fileContent, "File", fileName);
            content.Add(new StringContent(resultId.ToString()), "ResultId");

            var response = await httpClient.PostAsync("/api/v1/documents", content, ct);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DocumentDto>(ct)
                ?? throw new ExternalServiceException("Documents.API");
        }
    }
}
