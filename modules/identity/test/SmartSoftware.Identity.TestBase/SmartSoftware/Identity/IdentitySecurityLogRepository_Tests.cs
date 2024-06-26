﻿using System.Threading.Tasks;
using Shouldly;
using SmartSoftware.Modularity;
using Xunit;

namespace SmartSoftware.Identity;

public abstract class IdentitySecurityLogRepository_Tests<TStartupModule> : SmartSoftwareIdentityTestBase<TStartupModule>
    where TStartupModule : ISmartSoftwareModule
{
    protected IIdentitySecurityLogRepository RoleRepository { get; }
    protected IdentityTestData TestData { get; }

    protected IdentitySecurityLogRepository_Tests()
    {
        RoleRepository = GetRequiredService<IIdentitySecurityLogRepository>();
        TestData = GetRequiredService<IdentityTestData>();
    }

    [Fact]
    public async Task GetListAsync()
    {
        var logs = await RoleRepository.GetListAsync();
        logs.ShouldNotBeEmpty();
        logs.ShouldContain(x => x.ApplicationName == "Test-ApplicationName" && x.UserId == TestData.UserJohnId);
        logs.ShouldContain(x => x.ApplicationName == "Test-ApplicationName" && x.UserId == TestData.UserDavidId);
    }

    [Fact]
    public async Task GetCountAsync()
    {
        var count = await RoleRepository.GetCountAsync();
        count.ShouldBe(2);
    }
}
