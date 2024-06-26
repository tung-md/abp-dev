﻿using System;
using SmartSoftware.Data;
using SmartSoftware.Modularity;
using SmartSoftware.Uow;

namespace SmartSoftware.AuditLogging.MongoDB;

[DependsOn(
    typeof(SmartSoftwareAuditLoggingTestBaseModule),
    typeof(SmartSoftwareAuditLoggingMongoDbModule)
)]
public class SmartSoftwareAuditLoggingMongoDbTestModule : SmartSoftwareModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<SmartSoftwareDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = MongoDbFixture.GetRandomConnectionString();
        });
    }
}
