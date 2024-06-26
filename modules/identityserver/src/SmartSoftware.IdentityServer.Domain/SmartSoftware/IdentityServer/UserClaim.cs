﻿using JetBrains.Annotations;
using SmartSoftware.Domain.Entities;

namespace SmartSoftware.IdentityServer;

public abstract class UserClaim : Entity
{
    public virtual string Type { get; protected set; }

    protected UserClaim()
    {

    }

    protected UserClaim([NotNull] string type)
    {
        Check.NotNull(type, nameof(type));

        Type = type;
    }
}
