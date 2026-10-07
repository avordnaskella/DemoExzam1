using System;
using System.Collections.Generic;

namespace demka1.Models;

public partial class Size
{
    public int IdSize { get; set; }

    public float Size1 { get; set; }

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
