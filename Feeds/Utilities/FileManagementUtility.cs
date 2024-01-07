namespace Feeds.Utilities;

public class FileManagementUtility
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public FileManagementUtility(IWebHostEnvironment webHostEnvironment)
    {
        // Inject web host environment to the constructor to work with www root path
        _webHostEnvironment = webHostEnvironment;
    }

    public string UploadFile(IFormFile file, string targetPath)
    {
        string wwwRootPath = _webHostEnvironment.WebRootPath;
        if (file != null)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetFileNameWithoutExtension(file.FileName) +
                              Path.GetExtension(file.FileName);
            string filePath = Path.Combine(wwwRootPath, targetPath);

            if (!System.IO.Directory.Exists(filePath))
            {
                System.IO.Directory.CreateDirectory(filePath);
            }


            using (FileStream fileStream = new FileStream(Path.Combine(filePath, fileName), FileMode.Create))
            {
                // Create a new file stream to save the product image file to the path
                file.CopyTo(fileStream);
            }

            return @"/" + targetPath + @"/" + fileName;
        }

        return null;
    }

    public void RemoveFile(object modelFileProperty)
    {
        string wwwRootPath = _webHostEnvironment.WebRootPath;
        if (!string.IsNullOrEmpty(modelFileProperty.ToString()))
        {
            var oldFile = Path.Combine(wwwRootPath, modelFileProperty.ToString().TrimStart('/'));
            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }
        }
    }
}