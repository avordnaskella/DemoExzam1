using System;
using System.Collections.Generic;

namespace demka1.Models;

public partial class Size
{
    public int SizeId { get; set; }

    public double Name { get; set; }

    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
