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
using SmartSoftware.Blogging.Posts;

// ReSharper disable once CheckNamespace
namespace SmartSoftware.Blogging;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IPostAppService), typeof(PostsClientProxy))]
public partial class PostsClientProxy : ClientProxyBase<IPostAppService>, IPostAppService
{
    public virtual async Task<ListResultDto<PostWithDetailsDto>> GetListByBlogIdAndTagNameAsync(Guid blogId, string tagName)
    {
        return await RequestAsync<ListResultDto<PostWithDetailsDto>>(nameof(GetListByBlogIdAndTagNameAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), blogId },
            { typeof(string), tagName }
        });
    }

    public virtual async Task<ListResultDto<PostWithDetailsDto>> GetTimeOrderedListAsync(Guid blogId)
    {
        return await RequestAsync<ListResultDto<PostWithDetailsDto>>(nameof(GetTimeOrderedListAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), blogId }
        });
    }

    public virtual async Task<PostWithDetailsDto> GetForReadingAsync(GetPostInput input)
    {
        return await RequestAsync<PostWithDetailsDto>(nameof(GetForReadingAsync), new ClientProxyRequestTypeValue
        {
            { typeof(GetPostInput), input }
        });
    }

    public virtual async Task<PostWithDetailsDto> GetAsync(Guid id)
    {
        return await RequestAsync<PostWithDetailsDto>(nameof(GetAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }

    public virtual async Task<PostWithDetailsDto> CreateAsync(CreatePostDto input)
    {
        return await RequestAsync<PostWithDetailsDto>(nameof(CreateAsync), new ClientProxyRequestTypeValue
        {
            { typeof(CreatePostDto), input }
        });
    }

    public virtual async Task<PostWithDetailsDto> UpdateAsync(Guid id, UpdatePostDto input)
    {
        return await RequestAsync<PostWithDetailsDto>(nameof(UpdateAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id },
            { typeof(UpdatePostDto), input }
        });
    }

    public virtual async Task<List<PostWithDetailsDto>> GetListByUserIdAsync(Guid userId)
    {
        return await RequestAsync<List<PostWithDetailsDto>>(nameof(GetListByUserIdAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), userId }
        });
    }

    public virtual async Task<List<PostWithDetailsDto>> GetLatestBlogPostsAsync(Guid blogId, int count)
    {
        return await RequestAsync<List<PostWithDetailsDto>>(nameof(GetLatestBlogPostsAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), blogId },
            { typeof(int), count }
        });
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await RequestAsync(nameof(DeleteAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }
}