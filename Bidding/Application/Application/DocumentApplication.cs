using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using GoldBank.Application.IApplication;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Infrastructure.Infrastructure;
using GoldBank.Models;
using MySqlX.XDevAPI;
using System.Text.RegularExpressions;

namespace GoldBank.Application.Application
{
    public class DocumentApplication : IBaseApplication<Document>, IDocumentApplication
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _bucketUrl;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _publicKey;
        private static readonly HttpClient client = new HttpClient();
        public IDocumentInfrastructure DocumentInfrastructure { get; }

        public DocumentApplication(IDocumentInfrastructure DocumentInfrastructure, IConfiguration configuration, ILogger<Document> logger)
        {
            this.DocumentInfrastructure = DocumentInfrastructure;
            _bucketName = configuration["AWS:BucketName"];
            _bucketUrl = configuration["AWS:BucketUrl"];
            _accessKey = configuration["AWS:AccessKey"];
            _secretKey = configuration["AWS:SecretKey"];
            _publicKey = configuration["AWS:BucketPublicURL"];

            var config = new AmazonS3Config
            {
                ServiceURL = _bucketUrl,
                ForcePathStyle = true 
            };

            _s3Client = new AmazonS3Client(_accessKey, _secretKey, config);

        }
        
        public async Task<bool> Activate(Document entity)
        {
            return await DocumentInfrastructure.Activate(entity);
        }

        public async Task<int> Add(Document entity)
        {
            return await DocumentInfrastructure.Add(entity);
        }

        public async Task<Document> Get(Document entity)
        {
            return await DocumentInfrastructure.Get(entity);
        }

        public async Task<List<Document>> GetList(Document entity)
        {
            return await DocumentInfrastructure.GetList(entity);
        }

        public async Task<bool> Update(Document entity)
        {
            return await DocumentInfrastructure.Update(entity);
        }
        public Task<AllResponse<Document>> GetAll(AllRequest<Document> entity)
        {
            throw new NotImplementedException();
        }
        public async Task<int> UploadImage(Document Document)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(Document.File.FileName);
                string extension = Path.GetExtension(Document.File.FileName);
                var memoryStream = new MemoryStream();
                await Document.File.CopyToAsync(memoryStream);

                Document.Image64String = Convert.ToBase64String(memoryStream.ToArray());
                Document.Image64String = "data:image/png;base64," + Convert.ToBase64String(memoryStream.ToArray());


                if (string.IsNullOrWhiteSpace(Document.Image64String) || string.IsNullOrWhiteSpace(Document.Name))
                {
                    throw new InvalidOperationException("Missing base64File or filename");
                }

                var match = Regex.Match(Document.Image64String, @"^data:image\/(\w+);base64,(.+)$");
                if (!match.Success)
                {
                    throw new InvalidOperationException("Invalid base64 file format");
                }

                extension = match.Groups[1].Value;
                var base64Data = match.Groups[2].Value;
                var fileBytes = Convert.FromBase64String(base64Data);
                var contentType = $"image/{extension}";

                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = Document.Name,
                    InputStream = new MemoryStream(fileBytes),
                    ContentType = contentType,
                    AutoCloseStream = true
                };

                try
                {
                    var response = await _s3Client.PutObjectAsync(putRequest);
                    Document.Url =  $"{_publicKey}/{_bucketName}/{Document.Name}";
                    return await DocumentInfrastructure.Add(Document);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("S3 Upload Error: " + ex.Message);
                    throw new InvalidOperationException($"S3 upload failed: {ex.Message}", ex);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading Document.Image: {ex.Message}");
                throw; 
            }
        }
        public async Task<int> UploadFile(Document document)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(document.File.FileName);

                if (document.File == null || string.IsNullOrWhiteSpace(fileName))
                {
                    throw new ArgumentException("File and document name must be provided.");
                }

                var memoryStream = new MemoryStream();
                await document.File.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var contentType = "text/csv";

                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = fileName,
                    InputStream = memoryStream,
                    ContentType = contentType,
                    AutoCloseStream = true
                };

                try
                {
                    var response = await _s3Client.PutObjectAsync(putRequest);
                    var url = $"{_publicKey}/{_bucketName}/{fileName}";

                    // Optionally save document metadata in your infrastructure
                    document.Url = url;
                    var fPath = @"/var/lib/mysql-files/file.csv";
                    await DownloadFileAsync(url, fPath);
                    return await DocumentInfrastructure.Add(document);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"S3 upload failed: {ex.Message}", ex);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading Document.Image: {ex.Message}");
                throw;
            }
        }
        public static async Task DownloadFileAsync(string url, string destinationPath)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(destinationPath);
                
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    Console.WriteLine($"Directory created: {directoryPath}");
                }
                
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using (var fileStream = new FileStream(destinationPath, FileMode.Create))
                {
                    await response.Content.CopyToAsync(fileStream);
                }

                Console.WriteLine($"File downloaded successfully to {destinationPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
            }
        }
    }
}
