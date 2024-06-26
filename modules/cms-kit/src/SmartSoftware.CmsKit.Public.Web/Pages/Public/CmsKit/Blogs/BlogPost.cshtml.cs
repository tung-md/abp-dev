﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartSoftware.GlobalFeatures;
using SmartSoftware.CmsKit.Blogs;
using SmartSoftware.CmsKit.Contents;
using SmartSoftware.CmsKit.GlobalFeatures;
using SmartSoftware.CmsKit.Public.Blogs;
using SmartSoftware.CmsKit.Web.Contents;

namespace SmartSoftware.CmsKit.Public.Web.Pages.Public.CmsKit.Blogs;

public class BlogPostModel : CmsKitPublicPageModelBase
{
    [BindProperty(SupportsGet = true)]
    public string BlogSlug { get; set; }

    [BindProperty(SupportsGet = true)]
    public string BlogPostSlug { get; set; }

    public BlogPostViewModel ViewModel { get; private set; }

    public BlogFeatureDto CommentsFeature { get; private set; }

    public BlogFeatureDto ReactionsFeature { get; private set; }

    public BlogFeatureDto RatingsFeature { get; private set; }

    public BlogFeatureDto TagsFeature { get; private set; }

    public BlogFeatureDto BlogPostScrollIndexFeature { get; private set; }

    public BlogFeatureDto PreventXssFeature { get; private set; }

    protected IBlogPostPublicAppService BlogPostPublicAppService { get; }

    protected IBlogFeatureAppService BlogFeatureAppService { get; }

    protected ContentParser ContentParser { get; }

    public BlogPostModel(
        IBlogPostPublicAppService blogPostPublicAppService,
        IBlogFeatureAppService blogFeaturePublicAppService,
        ContentParser contentParser)
    {
        BlogPostPublicAppService = blogPostPublicAppService;
        BlogFeatureAppService = blogFeaturePublicAppService;
        ContentParser = contentParser;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        var blogPostPublicDto = await BlogPostPublicAppService.GetAsync(BlogSlug, BlogPostSlug);
        ViewModel = ObjectMapper.Map<BlogPostCommonDto, BlogPostViewModel>(blogPostPublicDto);
        if (ViewModel == null)
        {
            return NotFound();
        }
        
        ViewModel.ContentFragments = await ContentParser.ParseAsync(blogPostPublicDto.Content);

        if (GlobalFeatureManager.Instance.IsEnabled<CommentsFeature>())
        {
            CommentsFeature = await BlogFeatureAppService.GetOrDefaultAsync(ViewModel.BlogId, GlobalFeatures.CommentsFeature.Name);
        }

        if (GlobalFeatureManager.Instance.IsEnabled<ReactionsFeature>())
        {
            ReactionsFeature = await BlogFeatureAppService.GetOrDefaultAsync(ViewModel.BlogId, GlobalFeatures.ReactionsFeature.Name);
        }

        if (GlobalFeatureManager.Instance.IsEnabled<RatingsFeature>())
        {
            RatingsFeature = await BlogFeatureAppService.GetOrDefaultAsync(ViewModel.BlogId, GlobalFeatures.RatingsFeature.Name);
        }

        if (GlobalFeatureManager.Instance.IsEnabled<TagsFeature>())
        {
            TagsFeature = await BlogFeatureAppService.GetOrDefaultAsync(ViewModel.BlogId, GlobalFeatures.TagsFeature.Name);
        }

        if (GlobalFeatureManager.Instance.IsEnabled<BlogPostScrollIndexFeature>())
        {
            BlogPostScrollIndexFeature = await BlogFeatureAppService.GetOrDefaultAsync(ViewModel.BlogId, GlobalFeatures.BlogPostScrollIndexFeature.Name);
        }

        PreventXssFeature = await BlogFeatureAppService.GetOrDefaultAsync(ViewModel.BlogId, BlogConsts.PreventXssFeatureName);

        return Page();
    }
}
