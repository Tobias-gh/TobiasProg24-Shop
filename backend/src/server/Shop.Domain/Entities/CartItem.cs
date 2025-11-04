using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.Entities
{
    public class CartItem
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductId {get; set;}
        public int Quantity { get; set; } 
        public DateTime AddedAt { get; set; }

        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;


    }
}
