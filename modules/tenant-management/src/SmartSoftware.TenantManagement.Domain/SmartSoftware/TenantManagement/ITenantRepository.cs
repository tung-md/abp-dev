﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SmartSoftware.Domain.Repositories;

namespace SmartSoftware.TenantManagement;

public interface ITenantRepository : IBasicRepository<Tenant, Guid>
{
    Task<Tenant> FindByNameAsync(
        string normalizedName,
        bool includeDetails = true,
        CancellationToken cancellationToken = default);

    [Obsolete("Use FindByNameAsync method.")]
    Tenant FindByName(
        string normalizedName,
        bool includeDetails = true
    );

    [Obsolete("Use FindAsync method.")]
    Tenant FindById(
        Guid id,
        bool includeDetails = true
    );

    Task<List<Tenant>> GetListAsync(
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string filter = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(
        string filter = null,
        CancellationToken cancellationToken = default);
}
