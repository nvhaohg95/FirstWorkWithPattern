using Application.Interfaces;
using Application.Services;
using Domain.Models;
using FirstWorkWithPattern.Base;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FirstWorkWithPattern.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class ProductController : BaseController<ProductController>
    {
        private readonly ProductService _productService;
        public ProductController(ILogService<ProductController> log, ProductService productService) : base(log)
        {
            _productService = productService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> Create(Products product)
        {
            product.Id = Guid.NewGuid();
            _log.Info(JsonConvert.SerializeObject(product));
            var response = await _productService.Add(product);
            return Ok(product);
        }


        [HttpPost]
        [Route("update/{id}")]
        public async Task<IActionResult> Update(Guid id)
        {
            var response = await _productService.Update(id);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> Get(string search)
        {
            var response = _productService.Get(search);
            return Ok(response);
        }


    }
}
