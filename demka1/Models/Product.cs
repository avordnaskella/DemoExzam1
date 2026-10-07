using System;
using System.Collections.Generic;

namespace demka1.Models;

public partial class Product
{
    public int IdProduct { get; set; }

    public int IdCategory { get; set; }

    public int IdSubcategory { get; set; }

    public string? Description { get; set; }

    public string Structure { get; set; } = null!;

    public int Price { get; set; }

    public int IdManufacture { get; set; }

    public string Title { get; set; } = null!;

    public string? Image { get; set; }

    public virtual Category IdCategoryNavigation { get; set; } = null!;

    public virtual Manufacturer IdManufactureNavigation { get; set; } = null!;

    public virtual Subcategory IdSubcategoryNavigation { get; set; } = null!;

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();
}
