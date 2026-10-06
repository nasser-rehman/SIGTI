namespace SIGTI.Infrastructure.Storage
{
    public sealed class FileStorageOptions
    {
        public const string SectionName = "FileStorage";
        public string BasePath { get; set; } = "App_Data/Uploads";
    }
}
