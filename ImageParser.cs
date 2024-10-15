using System.Diagnostics;
using System.Text.Json;

namespace ImageParser
{
    public class ImageParser
    {
        private readonly string _baseDirectory;
        private readonly string[] _supportedExtensions = { ".png", ".svg" };
        private readonly Dictionary<string, Asset> _assets = new Dictionary<string, Asset>();

        private int _totalImagesFound = 0;
        private int _totalSVGs = 0;
        private int _totalPNGs = 0;
        private HashSet<string> _fileTypes = new HashSet<string>();

        public ImageParser(string baseDirectory)
        {
            _baseDirectory = baseDirectory;
        }

        public void ParseDirectory()
        {
            // Start tracking time
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var imageFiles = Directory.GetFiles(_baseDirectory, "*.*", SearchOption.AllDirectories)
                                       .Where(file => _supportedExtensions.Contains(Path.GetExtension(file)));

            foreach (var file in imageFiles)
            {
                _totalImagesFound++;
                ProcessFile(file);
            }

            // Stop tracking time
            stopwatch.Stop();

            SaveAssetsJson();

            // Display summary
            Console.WriteLine("Summary:");
            Console.WriteLine($"Total images found: {_totalImagesFound}");
            Console.WriteLine($"Total SVG files: {_totalSVGs}");
            Console.WriteLine($"Total PNG files: {_totalPNGs}");
            Console.WriteLine($"Total unique file types: {_fileTypes.Count}");
            Console.WriteLine($"Elapsed time: {stopwatch.Elapsed.TotalSeconds} seconds");
        }

        private void ProcessFile(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath).TrimStart('.');
            string relativePath = GetRelativePath(filePath, _baseDirectory);
            string size = ExtractSizeFromFileName(fileName);

            // Track specific file types
            _fileTypes.Add(extension);

            // Track number of SVGs and PNGs
            if (extension == "svg") _totalSVGs++;
            if (extension == "png") _totalPNGs++;

            string assetKey = $"{fileName}_{size}";

            if (!_assets.TryGetValue(assetKey, out var asset))
            {
                asset = new Asset
                {
                    Name = fileName.Replace('_', ' '),
                    Url = relativePath
                };
                _assets[assetKey] = asset;
            }

            if (!asset.SupportedFileTypes.Contains(extension))
            {
                asset.SupportedFileTypes.Add(extension);
            }

            if (!string.IsNullOrEmpty(size) && !asset.SupportedSizes.Contains(size))
            {
                asset.SupportedSizes.Add(size);
            }
        }

        private string ExtractSizeFromFileName(string fileName)
        {
            var sizeTokens = new[] { "16", "32", "64", "128", "256" };
            foreach (var token in sizeTokens)
            {
                if (fileName.Contains(token))
                {
                    return $"{token}x{token}";
                }
            }
            return string.Empty;
        }

        private string GetRelativePath(string fullPath, string baseDirectory)
        {
            return "/" + Path.GetRelativePath(baseDirectory, fullPath).Replace("\\", "/");
        }

        private void SaveAssetsJson()
        {
            string outputPath = Path.Combine(_baseDirectory, "assets.json");
            var options = new JsonSerializerOptions { WriteIndented = true };
            var assetsList = _assets.Values.ToList();
            string json = JsonSerializer.Serialize(assetsList, options);
            File.WriteAllText(outputPath, json);
            Console.WriteLine($"Assets.json generated at: {outputPath}");
        }
    }
}