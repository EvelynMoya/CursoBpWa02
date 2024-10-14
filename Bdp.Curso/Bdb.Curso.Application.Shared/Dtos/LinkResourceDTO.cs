using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bdb.Curso.Application.Shared.Dtos
{
    public class LinkResourceDTO
    {
        public string Href { get; set; }    // URL del enlace
        public string Rel { get; set; }     // Relación (e.g., "self", "update")
        public string Metodo { get; set; }  // Método HTTP (e.g., "GET", "POST")
    }
}
