using System;
using System.Collections.Generic;

namespace BaseApplication.Entity;

public partial class RegistrationEmail
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public bool IsActive { get; set; }
}
