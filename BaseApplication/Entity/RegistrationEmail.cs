using System;
using System.Collections.Generic;

namespace BaseApplication.Entity;

public partial class RegistrationEmail
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public int RoleId { get; set; }

    public bool IsActive { get; set; }

    public virtual Role Role { get; set; } = null!;
}
