using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace YouConf.Helpers
{
    public static class HtmlHelpers
    {
        public static string Truncate(this HtmlHelper helper, string input, int length)
        {
            if (input.Length <= length)
            {
                return input;
            }
            else
            {
                return input.Substring(0, length) + "...";
            }
        }

        //Inspiration from http://weblogs.asp.net/gunnarpeipman/archive/2012/06/17/asp-net-mvc-how-to-show-asterisk-after-required-field-label.aspx
        public static MvcHtmlString LabelWithRequiredAsteriskFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes = null)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, helper.ViewData);

            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            string labelText = metaData.DisplayName ?? metaData.PropertyName ?? htmlFieldName.Split('.').Last();

            if (String.IsNullOrEmpty(labelText))
                return MvcHtmlString.Empty;

            bool isRequired = false;

            if (metaData.ContainerType != null)
            {
                isRequired = metaData.ContainerType.GetProperty(metaData.PropertyName)
                                .GetCustomAttributes(typeof(RequiredAttribute), false)
                                .Length == 1;
            }
 

            var tag = new TagBuilder("label");
            tag.Attributes.Add("for", helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));

            if (isRequired)
                tag.Attributes.Add("class", "label-required");

            //if (!string.IsNullOrEmpty(metaData.Description))
            //    tag.Attributes.Add("title", metaData.Description);

            //if (!string.IsNullOrEmpty(metaData.DisplayName))
            //{
            //    var asteriskTag = new TagBuilder("img");
            //    asteriskTag.Attributes.Add("src", "/images/help.png");
            //    asteriskTag.Attributes.Add("class", "help-icon");
            //    asteriskTag.Attributes.Add("title", metaData.DisplayName);
            //    output += asteriskTag.ToString(TagRenderMode.SelfClosing);
            //}

            if (!string.IsNullOrEmpty(metaData.DisplayName))
                tag.Attributes.Add("title", metaData.DisplayName);

            tag.SetInnerText(labelText);

            tag.MergeAttributes(htmlAttributes, replaceExisting: true);
            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

    }
}