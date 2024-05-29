// This file is automatically generated by SS framework to use MVC Controllers from CSharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartSoftware;
using SmartSoftware.Application.Dtos;
using SmartSoftware.DependencyInjection;
using SmartSoftware.Http.Client;
using SmartSoftware.Http.Client.ClientProxying;
using SmartSoftware.Http.Modeling;
using SmartSoftware.CmsKit.Admin.Pages;

// ReSharper disable once CheckNamespace
namespace SmartSoftware.CmsKit.Admin.Pages;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IPageAdminAppService), typeof(PageAdminClientProxy))]
public partial class PageAdminClientProxy : ClientProxyBase<IPageAdminAppService>, IPageAdminAppService
{
    public virtual async Task<PageDto> GetAsync(Guid id)
    {
        return await RequestAsync<PageDto>(nameof(GetAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }

    public virtual async Task<PagedResultDto<PageDto>> GetListAsync(GetPagesInputDto input)
    {
        return await RequestAsync<PagedResultDto<PageDto>>(nameof(GetListAsync), new ClientProxyRequestTypeValue
        {
            { typeof(GetPagesInputDto), input }
        });
    }

    public virtual async Task<PageDto> CreateAsync(CreatePageInputDto input)
    {
        return await RequestAsync<PageDto>(nameof(CreateAsync), new ClientProxyRequestTypeValue
        {
            { typeof(CreatePageInputDto), input }
        });
    }

    public virtual async Task<PageDto> UpdateAsync(Guid id, UpdatePageInputDto input)
    {
        return await RequestAsync<PageDto>(nameof(UpdateAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id },
            { typeof(UpdatePageInputDto), input }
        });
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await RequestAsync(nameof(DeleteAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }

    public virtual async Task SetAsHomePageAsync(Guid id)
    {
        await RequestAsync(nameof(SetAsHomePageAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }
}