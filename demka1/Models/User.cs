using System;
using System.Collections.Generic;

namespace demka1.Models;

public partial class User
{
    public string Name { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string? Patronymic { get; set; }

    public int IdRole { get; set; }

    public virtual Role IdRoleNavigation { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
