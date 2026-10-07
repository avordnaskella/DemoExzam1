using System;
using System.Collections.Generic;

namespace demka1.Models;

public partial class Category
{
    public int IdCategory { get; set; }

    public string Title { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
