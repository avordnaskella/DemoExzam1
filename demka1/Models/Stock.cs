using System;
using System.Collections.Generic;

namespace demka1.Models;

public partial class Stock
{
    public int StockId { get; set; }

    public int ProductId { get; set; }

    public int SizeId { get; set; }

    public int Count { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Size Size { get; set; } = null!;
}
