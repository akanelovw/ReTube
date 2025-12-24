namespace ReTube.Service
{
    public class FileManagement
    {
        public static string UploadImage(IFormFile file, IWebHostEnvironment webHostEnvironment)
        {
            List<string> validExtensions = new List<string>() { ".jpg", ".png", ".gif" };
            string extension = Path.GetExtension(file.FileName);
            if (!validExtensions.Contains(extension))
            {
                return $"Extension is not valid ({string.Join(',', validExtensions)})";
            }

            long size = file.Length;

            if (size > (5 * 1024 * 1024))
            {
                return "Maximum size can be 5mb";
            }
            string fileName = Guid.NewGuid().ToString() + extension;
            string path = Path.Combine(webHostEnvironment.WebRootPath, "Uploads", "image");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create);
            file.CopyTo(stream);

            return fileName;
        }

        //public static string UpdateImage(IFormFile file, string file_type, string file_field, IWebHostEnvironment webHostEnvironment)
        //{
        //    string uniqueFileName = file_field;

        //    if (file != null)
        //    {
        //        string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, file_type);
        //        uniqueFileName = file.FileName;
        //        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
        //        if (!Directory.Exists(uploadsFolder))
        //        {
        //            Directory.CreateDirectory(uploadsFolder);
        //        }
        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            file.CopyTo(fileStream);
        //        }
        //    }
        //    return uniqueFileName;
        //}
    }
}
