using System;
using System.Collections.Generic;

namespace demka1.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int CategoryId { get; set; }

    public int SubcategoryId { get; set; }

    public int ManufacturerId { get; set; }

    public string Image { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Structure { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Manufacturer Manufacturer { get; set; } = null!;

    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();

    public virtual Subcategory Subcategory { get; set; } = null!;
}
