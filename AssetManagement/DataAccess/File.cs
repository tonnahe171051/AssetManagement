using System;
using System.Collections.Generic;

namespace AssetManagement.DataAccess;

public partial class File
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Path { get; set; } = null!;

    public DateTime UploadDate { get; set; }

    public int AssetId { get; set; }

    public int TagId { get; set; }

    public virtual Asset Asset { get; set; } = null!;

    public virtual Tag Tag { get; set; } = null!;
}
