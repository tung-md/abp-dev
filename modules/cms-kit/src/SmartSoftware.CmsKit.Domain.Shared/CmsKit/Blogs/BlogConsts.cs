﻿namespace SmartSoftware.CmsKit.Blogs;

public class BlogConsts
{
    public static int MaxNameLength { get; set; } = 64;

    public static int MaxSlugLength { get; set; } = 64;

    public const string PreventXssFeatureName = "CmsKit.BlogPost.PreventXssFeature";
}
