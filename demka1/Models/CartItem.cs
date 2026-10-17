using System;
using System.Collections.Generic;
using System.Text;

namespace demka1.Models
{
    public class CartItem
    {
        public Product Product { get; set; } = null!;
        public Size Size { get; set; } = null!;
        public int Count { get; set; }

        
        public string Display => $"{Product.Title} | размер {Size.Size1} | {Count} шт.";
    }
}
