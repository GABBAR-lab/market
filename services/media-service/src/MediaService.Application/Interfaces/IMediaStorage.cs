namespace MediaService.Application.Interfaces;

public interface IMediaStorage
{
    Task<(string FileName, string Url, long SizeBytes)> SaveAsync(Stream content, string category, string extension);
    Task DeleteFileAsync(string category, string fileName);
}
