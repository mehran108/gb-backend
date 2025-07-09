using ExcelDataReader;
using GoldBank.Application.Application;
using GoldBank.Application.IApplication;
using GoldBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;

namespace GoldBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class OrderController : ControllerBase
    {
        public OrderApplication OrderApplication { get; }
        public ILogger logger { get; set; }
    }
}
