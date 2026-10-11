using System;
using System.Collections.Generic;
using System.Text;

namespace demka1.Models
{
    public partial class User
    {
        public string FullName => $"{Lastname} {Name} {Patronymic}";
    }
}
