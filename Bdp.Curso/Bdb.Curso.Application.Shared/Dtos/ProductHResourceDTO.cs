using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bdb.Curso.Application.Shared.Dtos
{
    public class ProductHResourceDto
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? CategoryName { get; set; }

        public string? SupplierName { get; set; }


        public List<LinkResourceDto> Links { get; set; } = new();

    }
}
