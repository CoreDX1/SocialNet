using SocialNet.Application.Interfaces;

namespace SocialNet.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath;

    public LocalFileStorageService(string basePath = "wwwroot/uploads")
    {
        _basePath = basePath;
        if (!Directory.Exists(_basePath))
            Directory.CreateDirectory(_basePath);
    }

    public async Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        string folder,
        CancellationToken ct = default
    )
    {
        var folderPath = Path.Combine(_basePath, folder);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        var uniqueName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        var filePath = Path.Combine(folderPath, uniqueName);

        await using var target = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(target, ct);

        return $"/uploads/{folder}/{uniqueName}";
    }

    public void DeleteFile(string path)
    {
        var relativePath = path.TrimStart('/').Replace("uploads/", "");
        var fullPath = Path.Combine(_basePath, relativePath);
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }
}
