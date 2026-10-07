using System;
using System.Collections.Generic;

namespace demka1.Models;

public partial class Manufacturer
{
    public int IdManufacturer { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
