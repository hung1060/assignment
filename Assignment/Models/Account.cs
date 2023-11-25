using System;
using System.Collections.Generic;

namespace Assignment.Models
{
    public partial class Account
    {
        public string Id { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? CustomerId { get; set; }

        public virtual Customer? Customer { get; set; }
    }
}
