using Ensek.Api.Models;

namespace Ensek.Api.IntegrationTests;

public class UploadCsvResult
{
    public required HttpResponseMessage Response { get; set; }
    public UploadSuccessResponse? SuccessContent { get; set; }
    public UploadFailureResponse? FailureContent { get; set; }
    
    public class UploadSuccessResponse
    {
        public string Message { get; set; } = string.Empty;
        public int RecordCount { get; set; }
        public int SuccessRecordCount { get; set; }
        public int FailedRecordCount { get; set; }
        public List<CsvRowError> Errors { get; set; } = [];
    }

    public class UploadFailureResponse
    {
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = [];
    }
}