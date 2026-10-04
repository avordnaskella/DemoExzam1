using System;
using System.Collections.Generic;

namespace demka1.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public DateOnly DateOrder { get; set; }

    public string LoginId { get; set; } = null!;

    public virtual User Login { get; set; } = null!;

    public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();
}
