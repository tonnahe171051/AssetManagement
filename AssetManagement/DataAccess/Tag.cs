using System;
using System.Collections.Generic;

namespace AssetManagement.DataAccess;

public partial class Tag
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<File> Files { get; set; } = new List<File>();
}
