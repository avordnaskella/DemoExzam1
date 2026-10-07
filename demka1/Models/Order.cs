using System;
using System.Collections.Generic;

namespace demka1.Models;

public partial class Order
{
    public int IdOrder { get; set; }

    public DateOnly Date { get; set; }

    public string IdUser { get; set; } = null!;

    public virtual User IdUserNavigation { get; set; } = null!;
}
