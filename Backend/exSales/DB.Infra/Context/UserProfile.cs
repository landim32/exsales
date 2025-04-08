using System;
using System.Collections.Generic;

namespace DB.Infra.Context;

public partial class UserProfile
{
    public long ProfileId { get; set; }

    public long NetworkId { get; set; }

    public string Name { get; set; }

    public int Commission { get; set; }

    public virtual Network Network { get; set; }
}
