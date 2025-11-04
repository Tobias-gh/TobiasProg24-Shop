using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Domain.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }
        public string SessionId { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}
