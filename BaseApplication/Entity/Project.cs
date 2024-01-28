using System;
using System.Collections.Generic;

namespace BaseApplication.Entity;

public partial class Project
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
