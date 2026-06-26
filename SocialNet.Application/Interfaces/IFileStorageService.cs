namespace SocialNet.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        string folder,
        CancellationToken ct = default
    );
    void DeleteFile(string path);
}
