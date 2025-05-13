//using System.Text.Json;

//namespace MyAPI.Services
//{
//    public class FileService : IFileService
//    {
//        private readonly IHostEnvironment _environment;
//        private readonly string _webRootPath;
//        private readonly ILogger<FileService> _logger;

//        public FileService(IHostEnvironment environment, ILogger<FileService> logger = null)
//        {
//            _environment = environment;
//            _webRootPath = Path.Combine(_environment.ContentRootPath, "wwwroot");
//            _logger = logger;

//            if (!Directory.Exists(_webRootPath))
//            {
//                _logger?.LogWarning($"WebRootPath doesn't exist: {_webRootPath}");
//            }
//        }

//        public async Task<string> UploadAsync(IFormFile file, string folder)
//        {
//            if (file == null)
//                throw new ArgumentNullException(nameof(file), "File cannot be null");

//            try
//            {
//                ValidateFile(file);
//                folder = folder.Replace("\\", "/").TrimStart('/');

//                var extension = Path.GetExtension(file.FileName).ToLower();
//                var fileName = $"{Guid.NewGuid()}{extension}";
//                var relativeFilePath = $"/{folder}/{fileName}";
//                var uploadPath = Path.Combine(_webRootPath, folder);

//                if (!Directory.Exists(uploadPath))
//                {
//                    Directory.CreateDirectory(uploadPath);
//                    _logger?.LogInformation($"Created directory: {uploadPath}");
//                }

//                var fullPath = Path.Combine(uploadPath, fileName);
//                _logger?.LogInformation($"Uploading file to: {fullPath}");

//                using (var stream = new FileStream(fullPath, FileMode.Create))
//                {
//                    await file.CopyToAsync(stream);
//                }

//                _logger?.LogInformation($"File uploaded successfully: {relativeFilePath}");
//                return relativeFilePath;
//            }
//            catch (Exception ex)
//            {
//                _logger?.LogError(ex, $"Failed to upload file {file.FileName}: {ex.Message}");
//                throw new Exception($"File upload failed: {ex.Message}", ex);
//            }
//        }

//        public async Task<List<ImageDto>> UploadMultipleAsync(List<IFormFile> files, string folder)
//        {
//            if (files == null || !files.Any())
//            {
//                _logger?.LogWarning("No files provided for upload");
//                return new List<ImageDto>();
//            }

//            var results = new List<ImageDto>();
//            var failedFiles = new List<string>();

//            for (int i = 0; i < files.Count; i++)
//            {
//                try
//                {
//                    var file = files[i];
//                    if (file == null || file.Length == 0) continue;

//                    var filePath = await UploadAsync(file, folder);
//                    results.Add(new ImageDto
//                    {
//                        Path = filePath,
//                        Sort = i
//                    });
//                }
//                catch (Exception ex)
//                {
//                    _logger?.LogError(ex, $"Failed to upload file at index {i}: {ex.Message}");
//                    failedFiles.Add(files[i]?.FileName ?? $"File at index {i}");
//                }
//            }

//            if (failedFiles.Any() && results.Count == 0)
//                throw new Exception($"All files failed to upload: {string.Join(", ", failedFiles)}");

//            return results;
//        }

//        public string SerializeImages(List<ImageDto> images)
//        {
//            if (images == null || !images.Any())
//                return "[]";

//            try
//            {
//                return JsonSerializer.Serialize(images, new JsonSerializerOptions
//                {
//                    WriteIndented = false
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger?.LogError(ex, "Failed to serialize images to JSON");
//                throw new Exception("Failed to convert images to JSON format", ex);
//            }
//        }

//        public List<ImageDto> DeserializeImages(string json)
//        {
//            if (string.IsNullOrEmpty(json))
//                return new List<ImageDto>();

//            try
//            {
//                return JsonSerializer.Deserialize<List<ImageDto>>(json) ?? new List<ImageDto>();
//            }
//            catch (Exception ex)
//            {
//                _logger?.LogError(ex, $"Failed to deserialize JSON to images: {json}");
//                return new List<ImageDto>();
//            }
//        }

//        public async Task<bool> DeleteAsync(string filePath)
//        {
//            if (string.IsNullOrEmpty(filePath))
//                return false;

//            try
//            {
//                filePath = filePath.TrimStart('/');
//                var fullPath = Path.Combine(_webRootPath, filePath);

//                if (File.Exists(fullPath))
//                {
//                    File.Delete(fullPath);
//                    _logger?.LogInformation($"File deleted successfully: {filePath}");
//                    return true;
//                }
//                else
//                {
//                    _logger?.LogWarning($"File not found for deletion: {filePath}");
//                    return false;
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger?.LogError(ex, $"Failed to delete file: {filePath}");
//                return false;
//            }
//        }

//        private void ValidateFile(IFormFile file)
//        {
//            if (file == null || file.Length == 0)
//                throw new Exception("No file uploaded or file is empty");

//            if (file.Length > FileConstants.MaxFileSize)
//                throw new Exception($"File size exceeds {FileConstants.MaxFileSize / (1024 * 1024)}MB limit");

//            var extension = Path.GetExtension(file.FileName).ToLower();
//            if (!FileConstants.AllowedImageExtensions.Contains(extension))
//                throw new Exception($"Invalid file type. Allowed extensions: {string.Join(", ", FileConstants.AllowedImageExtensions)}");

//            if (!FileConstants.AllowedImageMimeTypes.Contains(file.ContentType))
//                throw new Exception($"Invalid content type: {file.ContentType}");
//        }
//    }
//}
