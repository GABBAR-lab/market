using MediaService.Application.Interfaces;

namespace MediaService.Infrastructure.Storage;

public class LocalMediaStorage : IMediaStorage
{
    private readonly string _wwwroot;

    public LocalMediaStorage(string wwwrootPath)
    {
        _wwwroot = wwwrootPath;
    }

    public async Task<(string FileName, string Url, long SizeBytes)> SaveAsync(Stream content, string category, string extension)
    {
        var dir = Path.Combine(_wwwroot, "uploads", category);
        Directory.CreateDirectory(dir);

        var fileName = $"{Guid.NewGuid():N}.webp";
        var path = Path.Combine(dir, fileName);

        await using var outStream = File.Create(path);
        await content.CopyToAsync(outStream);

        var size = new FileInfo(path).Length;
        return (fileName, $"/uploads/{category}/{fileName}", size);
    }

    public Task DeleteFileAsync(string category, string fileName)
    {
        var path = Path.Combine(_wwwroot, "uploads", category, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }
}
