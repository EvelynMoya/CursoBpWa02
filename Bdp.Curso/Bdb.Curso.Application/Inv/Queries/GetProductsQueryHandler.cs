using AutoMapper;
using Bdb.Curso.Application.Shared.Dtos;
using Bdb.Curso.Core.Entities;
using Bdb.Curso.EntityFrameworkCore;
using MailKit.Search;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bdb.Curso.Application.Inv.Queries
{
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
    {
        private readonly IGenericRepository<Product> _productsRepository;

        public GetProductsQueryHandler(
            IGenericRepository<Product> productsRepository)
        {
            _productsRepository = productsRepository;

        }

        public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {

            int pageSize = 10; // definir como politica

            IQueryable<ProductDto> query;
            List<ProductDto> paginatedProducts = new();

                                   
            try
            {
                query = _productsRepository
                       .Include(p => p.Category)           // utilizamos la relacion foranea
                       .Include(p => p.Supplier)
                       .Where(p => p.Name.Contains(request.SearchTerm ?? string.Empty))
                       .OrderBy(p => p.Name)
                       .Select(p => new ProductDto
                       {
                           Id = p.Id,
                           Name = p.Name,
                           CategoryName = p.Category.Name,
                           SupplierName = p.Supplier.Name
                       }
                       );

                paginatedProducts = await query
                  .Skip((request.PageNumber - 1) * pageSize)  // Omite los registros anteriores según la página actual
                  .Take(pageSize)  // Toma solo el número de registros especificado por el tamaño de la página
                  .ToListAsync(cancellationToken);  // Ejecuta la consulta y convierte el resultado en una lista
            }
            catch
            {


            }                                       
            return paginatedProducts;

        }

    }
}
