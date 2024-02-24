using System;
using System.Collections.Generic;

namespace BaseApplication.Entity;

public partial class Coordinator
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<TridentClient> TridentClients { get; set; } = new List<TridentClient>();
}
