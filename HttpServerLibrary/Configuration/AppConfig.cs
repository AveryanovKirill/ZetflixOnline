using System.Text.Json;

namespace HttpServerLibrary.Configuration
{
    public static class AppConfig
    {
        public static string Domain { get; set; } = "localhost";
        public static uint Port { get; set; } = 6529;
        public static string StaticDirectoryPath { get; set; } = @"public\";

        public static async Task LoadFromFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Файл '{filePath}' не найден.");
            }

            string jsonString = await File.ReadAllTextAsync(filePath);

            var config = JsonSerializer.Deserialize<Config>(jsonString);

            if (config != null)
            {
                Domain = config.Domain;
                Port = config.Port;
                StaticDirectoryPath = config.StaticDirectoryPath;
            }
            else
            {
                throw new InvalidOperationException("Не удалось десериализовать JSON файл.");
            }
        }

        private class Config
        {
            public string Domain { get; set; } = "localhost";
            public uint Port { get; set; } = 6529;
            public string StaticDirectoryPath { get; set; } = @"public\";
        }
    }
}

