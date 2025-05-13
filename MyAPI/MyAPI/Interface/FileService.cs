namespace MyAPI.Interface
{
    public interface IFileService
    {
        Task<string> UploadAsync(IFormFile file, string folder); // 👈 Thêm dòng này
        Task<List<string>> UploadMultipleAsync(List<IFormFile> files, string folder);
        string SerializeImages(List<string> images);
        List<string> DeserializeImages(string json);
        Task<bool> DeleteAsync(string filePath);
    }
}
