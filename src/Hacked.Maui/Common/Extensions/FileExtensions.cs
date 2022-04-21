namespace Hacked.Maui.Common.Extensions;

public static class FileExtensions
{
    private static readonly string LocalFolder;

    static FileExtensions()
    {
        // Gets the target platform's valid save location
        LocalFolder = FileSystem.AppDataDirectory;
    }

    public static string SaveTextToFile(string text, string fileName)
    {
        // Use Combine so that the correct file path slashes are used
        var filePath = Path.Combine(LocalFolder, fileName);

        if (File.Exists(filePath))
            File.Delete(filePath);

        File.WriteAllText(filePath, text);

        return filePath;
    }

    public static string LoadTextFromFile(string fileName)
    {
        // Use Combine so that the correct file path slashes are used
        var filePath = Path.Combine(LocalFolder, fileName);

        return File.Exists(filePath) ? File.ReadAllText(filePath) : null;
    }

    public static async Task<Stream> LoadStreamFromFileAsync(string filePath)
    {
        return await Task.Run(() =>
        {
            using var fileStream = File.OpenRead(filePath);
            return fileStream;
        });
    }

    public static async Task<byte[]> LoadBytesFromFileAsync(string filePath)
    {
        return await Task.Run(() => File.ReadAllBytes(filePath));
    }

    public static async Task<string> SaveStreamToFileAsync(this Stream dataStream, string fileName)
    {
        // Use Combine so that the correct file path slashes are used
        var filePath = Path.Combine(LocalFolder, fileName);

        if (File.Exists(filePath))
            File.Delete(filePath);

        await using var fileStream = File.OpenWrite(filePath);

        if (dataStream.CanSeek)
            dataStream.Position = 0;

        await dataStream.CopyToAsync(fileStream);

        return filePath;
    }

    public static async Task<string> SaveBytesToFileAsync(this byte[] dataBytes, string fileName)
    {
        return await Task.Run(() =>
        {
            // Use Combine so that the correct file path slashes are used
            var filePath = Path.Combine(LocalFolder, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);

            File.WriteAllBytes(filePath, dataBytes);

            return filePath;
        });
    }
}