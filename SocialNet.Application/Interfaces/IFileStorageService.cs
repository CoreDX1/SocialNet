namespace SocialNet.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(Stream file, string folder, CancellationToken ct = default);
    void DeleteFile(string path);
}
