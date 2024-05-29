﻿using Microsoft.AspNetCore.Razor.TagHelpers;
using SmartSoftware.AspNetCore.Mvc.UI.Bootstrap.Microsoft.AspNetCore.Razor.TagHelpers;

namespace SmartSoftware.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.ListGroup;

public class SmartSoftwareListGroupItemTagHelperService : SmartSoftwareTagHelperService<SmartSoftwareListGroupItemTagHelper>
{
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        MakeLinkIfHrefIsSet();
        SetTagNameAndAttributes(context, output);
    }

    protected virtual void SetTagNameAndAttributes(TagHelperContext context, TagHelperOutput output)
    {
        SetCommonTagNameAndAttributes(context, output);

        if (TagHelper.TagType == SmartSoftwareListItemTagType.Default)
        {
            output.TagName = "li";
        }
        else if (TagHelper.TagType == SmartSoftwareListItemTagType.Link)
        {
            output.TagName = "a";
            output.Attributes.AddClass("list-group-item-action");
            output.Attributes.Add("href", TagHelper.Href);
        }
        else if (TagHelper.TagType == SmartSoftwareListItemTagType.Button)
        {
            output.TagName = "button";
            output.Attributes.AddClass("list-group-item-action");
        }

    }

    protected virtual void SetCommonTagNameAndAttributes(TagHelperContext context, TagHelperOutput output)
    {
        output.Attributes.AddClass("list-group-item");

        if (TagHelper.Active ?? false)
        {
            output.Attributes.AddClass("active");
        }

        if (TagHelper.Disabled ?? false)
        {
            output.Attributes.AddClass("disabled");
        }

        if (TagHelper.Type != SmartSoftwareListItemType.Default)
        {
            output.Attributes.AddClass("list-group-item-" + TagHelper.Type.ToString().ToLowerInvariant());
        }
    }

    protected virtual void MakeLinkIfHrefIsSet()
    {
        if (!string.IsNullOrWhiteSpace(TagHelper.Href))
        {
            TagHelper.TagType = SmartSoftwareListItemTagType.Link;
        }
    }
}