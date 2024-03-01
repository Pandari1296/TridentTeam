using System;
using System.Collections.Generic;

namespace BaseApplication.Entity;

public partial class TridentClient
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int CoordinatorId { get; set; }

    public bool? Status { get; set; }

    public virtual Coordinator Coordinator { get; set; } = null!;
    public string? AlternatePhone { get; set; }
    public string? Notes { get; set; }
    public string? ZipCode { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? Address1 { get; set; }
}
