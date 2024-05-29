﻿using System;
using NuGet.Versioning;
using SmartSoftware.Cli.ProjectBuilding.Building.Steps;
using SmartSoftware.Cli.ProjectBuilding.Templates;
using SmartSoftware.Cli.ProjectBuilding.Templates.App;
using SmartSoftware.Cli.ProjectBuilding.Templates.Microservice;
using SmartSoftware.Cli.ProjectBuilding.Templates.MvcModule;

namespace SmartSoftware.Cli.ProjectBuilding.Building;

public static class TemplateProjectBuildPipelineBuilder
{
    public static ProjectBuildPipeline Build(ProjectBuildContext context)
    {
        var pipeline = new ProjectBuildPipeline(context);

        pipeline.Steps.Add(new FileEntryListReadStep());

        if (SemanticVersion.Parse(context.TemplateFile.Version) > new SemanticVersion(4, 3, 99))
        {
            pipeline.Steps.Add(new CreateAppSettingsSecretsStep());
        }

        pipeline.Steps.AddRange(context.Template.GetCustomSteps(context));

        pipeline.Steps.Add(new ProjectReferenceReplaceStep());
        pipeline.Steps.Add(new TemplateCodeDeleteStep());
        pipeline.Steps.Add(new SolutionRenameStep());

        if (context.Template.IsPro())
        {
            pipeline.Steps.Add(new LicenseCodeReplaceStep()); // todo: move to custom steps?
        }

        if (context.Template.Name == AppTemplate.TemplateName ||
            context.Template.Name == AppProTemplate.TemplateName)
        {
            pipeline.Steps.Add(new DatabaseManagementSystemChangeStep(context.Template.As<AppTemplateBase>().HasDbMigrations)); // todo: move to custom steps?
        }

        if (context.Template.Name == AppNoLayersTemplate.TemplateName ||
            context.Template.Name == AppNoLayersProTemplate.TemplateName)
        {
            pipeline.Steps.Add(new AppNoLayersDatabaseManagementSystemChangeStep()); // todo: move to custom steps?
        }

        if (context.Template.Name == ModuleTemplate.TemplateName ||
            context.Template.Name == ModuleProTemplate.TemplateName)
        {
            pipeline.Steps.Add(new AppModuleDatabaseManagementSystemChangeStep()); // todo: move to custom steps?
        }

        if ((context.BuildArgs.UiFramework == UiFramework.Mvc || context.BuildArgs.UiFramework == UiFramework.Blazor || context.BuildArgs.UiFramework == UiFramework.BlazorServer || context.BuildArgs.UiFramework == UiFramework.BlazorWebApp)
            && context.BuildArgs.MobileApp == MobileApp.None && context.Template.Name != MicroserviceProTemplate.TemplateName
            && context.Template.Name != MicroserviceServiceProTemplate.TemplateName)
        {
            pipeline.Steps.Add(new RemoveRootFolderStep());
        }

        pipeline.Steps.Add(new CheckRedisPreRequirements());

        pipeline.Steps.Add(new CreateProjectResultZipStep());

        return pipeline;
    }
}