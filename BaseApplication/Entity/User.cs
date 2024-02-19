using System;
using System.Collections.Generic;

namespace BaseApplication.Entity;

public partial class User
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? UserEmail { get; set; }

    public string? Password { get; set; }

    public int RoleId { get; set; }

    public string? Mobile { get; set; }

    public bool? IsActive { get; set; }

    public virtual Role Role { get; set; } = null!;
}
