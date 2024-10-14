using AutoMapper;
using Bdb.Curso.Application.Shared;
using Bdb.Curso.Application.Shared.Dtos;
using Bdb.Curso.HttpApi.Host.Authorization;
using Bdb.Curso.HttpApi.Host.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Utilities;
using System.Data;
 

namespace Bdb.Curso.HttpApi.Host.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class OnlineOrderHyperController : ControllerBase
    {
        private readonly IInvAppServices _invAppServices;

        private readonly CacheService _cacheService;

        private readonly LinkGenerator _linkGenerator;
        private readonly IMapper _mapper;

        public OnlineOrderHyperController(IInvAppServices invAppServices, CacheService cacheService,
            LinkGenerator linkGenerator, IMapper mapper
            )
        {                           
            _invAppServices = invAppServices;
            _cacheService = cacheService;
            _linkGenerator = linkGenerator;
            _mapper = mapper;

        }
                                   

        // GET: api/lista-productos
        [HttpGet("lista-productos")]

       // [CustomAuthorize(AppPermissions.Pages_Query_Products)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts([FromQuery] 
        GetProductRequest input)
        {
            //Manejo del caching
            string cacheKey = "GetProducts";
            var dataChache = _cacheService.Get<List<ProductDto>>(cacheKey);

            List<ProductDto> data;

            if(dataChache != null)
            {
                data = dataChache;
            }
            else
            {
                data = await _invAppServices.GetProducts(input.searchTerm, input.pageNumber);

                _cacheService.Set(cacheKey, data, TimeSpan.FromMinutes(1));

            }

            //completar HATEOAS
            var resourceList = new List<ProductHResourceDto>();

            ProductHResourceDto itemHATEOAS;

            foreach (var item in data)
            {
                itemHATEOAS = new();
                  itemHATEOAS = CreateProductResource(item);

                resourceList.Add(itemHATEOAS);
            }

                        
            if (data.Count == 0)
                return StatusCode(StatusCodes.Status204NoContent, ResponseApiService.Response(StatusCodes.Status204NoContent));

            //return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, resourceList));
            //return Ok(data);

            return Ok(new { Products = resourceList  });

        }

        private ProductHResourceDto CreateProductResource(ProductDto item)
        {
            var links = GetProductsLinks(item.Id);

            return new ProductHResourceDto
            {
                Id = item.Id,
                CategoryName = item.CategoryName,
                Name = item.Name,
                SupplierName = item.SupplierName,
                Links = links
            };

        }

        private List<LinkResourceDto> GetProductsLinks(int id)
        {


            // esta seccion debería construirse en un Helper especilizado
            var links = new List<LinkResourceDto>
            {
                  new LinkResourceDto
                {
                    Href = _linkGenerator.GetPathByAction(action: "GetProductById", controller: "OnlineOrderHyper", values: new { id }),
                    Rel = "self",
                    Metodo = "GET"
                } 

            };


            return links;
        }


        // Get a product by its ID with HATEOAS
        [HttpGet("GetProductById/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _invAppServices.GetProductDetailsByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Create the HATEOAS resource for this specific product
            var resource = CreateProductResource(_mapper.Map<ProductDto>(product));

            return Ok(resource);
        }


        // GET: api/lista-productos
        [HttpGet("lista-productos10")]

        //[CustomAuthorize(AppPermissions.Pages_Query_Products)]
        //[Authorize("Usuarios")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts10([FromQuery]
        GetProductRequest input)
        {
            //Manejo del caching
            string cacheKey = "GetProducts";
            var dataChache = _cacheService.Get<List<ProductDto>>(cacheKey);

            List<ProductDto> data;

            if (dataChache != null)
            {
                data = dataChache;
            }
            else
            {
                data = await _invAppServices.GetProducts(input.searchTerm, input.pageNumber);

                _cacheService.Set(cacheKey, data, TimeSpan.FromMinutes(1));

            }


            if (data.Count == 0)
                return StatusCode(StatusCodes.Status204NoContent, ResponseApiService.Response(StatusCodes.Status204NoContent));

            return StatusCode(StatusCodes.Status200OK, ResponseApiService.Response(StatusCodes.Status200OK, data));
            //return Ok(data);

        }





        [HttpPost("Registro")]
        public async Task<IActionResult> InventMov([FromBody] ProductMovRequest request)
        {
            var data = await _invAppServices.InventMov(request);
          
            return Ok(data);
        }
                                   

        //[HttpPost("RegistroSp")]
        //public async Task<IActionResult> InventMovSp([FromBody] ProductMovRequest request)
        //{
        //    var data =await _invAppServices.InventMovSp(request);
        //    return Ok(data);
        //}



    }



}
