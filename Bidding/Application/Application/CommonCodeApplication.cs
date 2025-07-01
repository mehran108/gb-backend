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
    public class CommonCodeApplication : IBaseApplication<CommonCode>, ICommonCodeApplication
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _bucketUrl;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _publicKey;
        public CommonCodeApplication(ICommonCodeInfrastructure CommonCodeInfrastructure, IConfiguration configuration, ILogger<CommonCode> logger)
        {
            this.CommonCodeInfrastructure = CommonCodeInfrastructure;
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

        public ICommonCodeInfrastructure CommonCodeInfrastructure { get; }
        public async Task<bool> Activate(CommonCode entity)
        {
            return await CommonCodeInfrastructure.Activate(entity);
        }

        public async Task<int> Add(CommonCode entity)
        {
            return await CommonCodeInfrastructure.Add(entity);
        }

        public async Task<CommonCode> Get(CommonCode entity)
        {
            return await CommonCodeInfrastructure.Get(entity);
        }

        public async Task<List<CommonCode>> GetList(CommonCode entity)
        {
            return await CommonCodeInfrastructure.GetList(entity);
        }

        public async Task<bool> Update(CommonCode entity)
        {
            return await CommonCodeInfrastructure.Update(entity);
        }
        public async Task<int> UploadImage(CommonCode commonCode)
        {
            try
            {
                string fileName = Path.GetFileNameWithoutExtension(commonCode.File.FileName);
                string extension = Path.GetExtension(commonCode.File.FileName);
                var memoryStream = new MemoryStream();
                await commonCode.File.CopyToAsync(memoryStream);

                commonCode.Image64String = Convert.ToBase64String(memoryStream.ToArray());
                commonCode.Image64String = "data:image/png;base64," + Convert.ToBase64String(memoryStream.ToArray());


                if (string.IsNullOrWhiteSpace(commonCode.Image64String) || string.IsNullOrWhiteSpace(commonCode.FileName))
                {
                    throw new InvalidOperationException("Missing base64File or filename");
                }

                var match = Regex.Match(commonCode.Image64String, @"^data:image\/(\w+);base64,(.+)$");
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
                    Key = commonCode.FileName,
                    InputStream = new MemoryStream(fileBytes),
                    ContentType = contentType,
                    AutoCloseStream = true
                };

                try
                {
                    var response = await _s3Client.PutObjectAsync(putRequest);
                    commonCode.DocumentPath =  $"{_publicKey}/{_bucketName}/{commonCode.FileName}";
                    return await CommonCodeInfrastructure.Add(commonCode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("S3 Upload Error: " + ex.Message);
                    throw new InvalidOperationException($"S3 upload failed: {ex.Message}", ex);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading commonCode.Image: {ex.Message}");
                throw; 
            }
        }

    }
}
