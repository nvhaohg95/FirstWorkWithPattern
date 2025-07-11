using Application.Services;
using Domain.Models;
using FirstWorkWithPattern.Base;
using Microsoft.AspNetCore.Mvc;

namespace FirstWorkWithPattern.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class ProductController : BaseController
    {
        private readonly ProductService _productService;
        public ProductController(ProductService productService)
        {
           _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            var response = await _productService.Add(product);
            return Ok(product);
        }

        [HttpGet]
        public async Task<IActionResult> Get(string search)
        {
            var response = _productService.Get(search);
            return Ok(response);
        }
    }
}
