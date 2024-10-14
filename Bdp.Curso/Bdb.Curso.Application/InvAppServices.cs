using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data;
using Bdb.Curso.Application.Shared;
using Bdb.Curso.Application.Shared.Dtos;
 
using Bdb.Curso.Core.Entities;
using Bdb.Curso.EntityFrameworkCore;
using MediatR;
using Bdb.Curso.Application.Inv.Queries;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Bdb.Curso.Application.Inv.Commands;

namespace Bdb.Curso.Application
{
    public class InvAppServices : IInvAppServices
    {                     
        private readonly IMapper _mapper;        
        private readonly IMediator _mediator; // Usaremos MediatR para orquestar los commands y queries
        private readonly IGenericRepository<Product> _productsRepository;


        public InvAppServices( IMapper mapper,  IMediator mediator , IGenericRepository<Product>  productsRepository
            )
        {                                 
            _mapper = mapper;           
            _mediator = mediator;          
            _productsRepository = productsRepository;
        }
                                        
        public async Task<List<ProductDTO>> GetProducts(string searchTerm, int pageNumber = 1)
        {    
            var query = new GetProductsQuery { SearchTerm = searchTerm, PageNumber = pageNumber };
            return await _mediator.Send(query);
          
        }

        public async Task<bool> InventMov(ProductMovRequest request)
        {
            // var command = _mapper.Map<ProductMovRequestCommand>(request);

            var command = new ProductMovRequestCommand
            {
                ProductId = request.ProductId,
                TypeId = request.TypeId,
                Amount = request.Amount,
                UserId = request.UserId

            };

            return await _mediator.Send(command);
        }


        public async Task<ProductDTO> GetProductDetailsByIdAsync(int id)
        {
            // Obtener el producto y los datos relacionados en una sola consulta
            var productDto = await _productsRepository 
                .Where(p => p.Id == id)
                .Include(p => p.Category)   // Cargar la categoría relacionada
                .Include(p => p.Supplier)   // Cargar el proveedor relacionado
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,  // inspecciona la autorizacion y si le envio
                    CategoryName = string.IsNullOrWhiteSpace(p.Category.Name) ? "No Category" : p.Category.Name, // Validar y proporcionar valor predeterminado
                    SupplierName = string.IsNullOrWhiteSpace(p.Supplier.Name) ? "No Supplier" : p.Supplier.Name  // Validar y proporcionar valor predeterminado
                })
                .FirstOrDefaultAsync();


            if (productDto == null)
            {
                return new ProductDTO();
            }


            return productDto;
        }



        //public async Task<bool> InventMovSp(ProductMovRequest request)
        //{
        //    bool result = false;

        //    if (request == null || request.Amount == 0) return false;

        //    //crear parametros
        //    var parameter = new[]
        //    {
        //        new SqlParameter("@ProductId",SqlDbType.Int){Value = request.ProductId},
        //         new SqlParameter("@TypeId",SqlDbType.Int){Value = request.TypeId},
        //          new SqlParameter("@UserId",SqlDbType.Int){Value = request.UserId},
        //           new SqlParameter("@Amount",SqlDbType.Decimal){Value = request.Amount}
        //    };

        //    //ejecutar
        //    try
        //    {
        //        await _productsRepository
        //                 .ExecuteStoredProcedureAsync("EXEC InsertProductMovement @ProductId,@TypeId,@UserId,@Amount"
        //                                             , parameter);
        //        result = true;
        //    }
        //    catch (Exception eex)
        //    {


        //    }

        //    return result;
        //}


    }
}
