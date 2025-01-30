namespace AidScript;

public static class FileReader
{
    public static string ReadFile(string filePath)
    {
        return File.ReadAllText(filePath);
    }
}
