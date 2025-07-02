using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using GoldBank.Application.IApplication;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
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

        public IDocumentInfrastructure DocumentInfrastructure { get; }
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

    }
}
