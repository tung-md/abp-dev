﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSoftware.PermissionManagement;

public interface IPermissionDataSeeder
{
    Task SeedAsync(
        string providerName,
        string providerKey,
        IEnumerable<string> grantedPermissions,
        Guid? tenantId = null
    );
}
