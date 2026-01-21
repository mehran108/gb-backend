using GoldBank.Application.IApplication;
using GoldBank.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoldBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        public IDocumentApplication DocumentApplication { get; }
        public ILogger logger { get; set; }
        public DocumentController(IConfiguration configuration, ILogger<DocumentController> logger, IDocumentApplication DocumentApplication)
        {
            this.DocumentApplication = DocumentApplication;
            this.logger = logger;
        }


        [HttpPost("Add")]
        public async Task<int> Add(Document Document)
        {
            return await DocumentApplication.Add(Document);
        }

        [HttpPost("Update")]
        public async Task<bool> Update(Document Document)
        {
            return await DocumentApplication.Update(Document);
        }

        [HttpPost("Delete")]
        public async Task<bool> Delete([FromBody]Document document)
        {
            return await DocumentApplication.Delete(document.DocumentIds);
        }
        [HttpGet("Get")]
        public async Task<Document> GetById([FromQuery] int DocumentId)
        {
            var Document = new Document { DocumentId = DocumentId };
            return await DocumentApplication.Get(Document);
        }
        [HttpPost("UploadImage")]
        public async Task<int> UploadImage(Document Document)
        {
            return await DocumentApplication.UploadImage(Document);
        }
        [HttpPost("UploadFile")]
        public async Task<int> UploadFile(Document document)
        {
            return await DocumentApplication.UploadFile(document);
        }
    }
}
