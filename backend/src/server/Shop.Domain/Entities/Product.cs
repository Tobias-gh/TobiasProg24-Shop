using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name {get; set; } = null!;
        public string? Description {get; set; } = null!;
        public decimal Price {get; set; }
        public int Stock {get; set; }
        public Guid CategoryId {get; set; }
        public Category? Category {get; set; }

    }
}
