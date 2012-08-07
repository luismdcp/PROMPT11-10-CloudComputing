using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Routing;

namespace CloudNotes.WebRole.Helpers
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString DisplayForValidationErrors(this HtmlHelper htmlHelper, IEnumerable<ValidationResult> validationErrors, object htmlAttributes = null)
        {
            var container = new TagBuilder("div");
            container.AddCssClass("alert alert-error");

            if (htmlAttributes != null)
            {
                container.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            }

            var list = new TagBuilder("ul");

            foreach (var validationResult in validationErrors)
            {
                var listItem = new TagBuilder("li");
                listItem.SetInnerText(htmlHelper.Raw(validationResult.ErrorMessage).ToString());
                list.InnerHtml += listItem;
            }

            container.InnerHtml = list.ToString();
            return MvcHtmlString.Create(container.ToString());
        }

        public static MvcHtmlString DisplayForAlertMessage(this HtmlHelper htmlHelper, string alertMessage, object htmlAttributes = null)
        {
            var container = new TagBuilder("div");
            container.AddCssClass("alert alert-info");

            if (htmlAttributes != null)
            {
                container.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            }

            container.InnerHtml = htmlHelper.Raw(alertMessage).ToString();
            return MvcHtmlString.Create(container.ToString());
        }

        public static Fieldset BeginFieldset(this HtmlHelper htmlHelper, string legend = null, object htmlAttributes = null)
        {
            var fieldsetTag = new TagBuilder("fieldset");

            if (htmlAttributes != null)
            {
                fieldsetTag.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            }

            htmlHelper.ViewContext.Writer.WriteLine(fieldsetTag.ToString(TagRenderMode.StartTag));

            if (legend != null)
            {
                var legendTag = new TagBuilder("legend");
                legendTag.SetInnerText(htmlHelper.Raw(legend).ToString());
                htmlHelper.ViewContext.Writer.WriteLine(legendTag.ToString());
            }

            return new Fieldset(htmlHelper.ViewContext);
        }

        public static MvcHtmlString Label(this HtmlHelper htmlHelper, string label, string @for, object htmlAttributes = null)
        {
            var labelTag = new TagBuilder("label");
            labelTag.MergeAttribute("for", @for);

            if (htmlAttributes != null)
            {
                labelTag.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            }
            
            labelTag.SetInnerText(htmlHelper.Raw(label).ToString());
            return MvcHtmlString.Create(labelTag.ToString());
        }

        public static MvcHtmlString Button(this HtmlHelper htmlHelper, string type, string text, object htmlAttributes)
        {
            var buttonTag = new TagBuilder("button");
            buttonTag.MergeAttribute("type", type);

            if (htmlAttributes != null)
            {
                buttonTag.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            }

            buttonTag.SetInnerText(htmlHelper.Raw(text).ToString());
            return MvcHtmlString.Create(buttonTag.ToString());
        }

        public static MvcHtmlString Hidden(this HtmlHelper htmlHelper, string id, string value)
        {
            var hiddenInputTag = new TagBuilder("input");
            hiddenInputTag.GenerateId(id);
            hiddenInputTag.MergeAttribute("type", "hidden");
            hiddenInputTag.MergeAttribute("value", value);

            return MvcHtmlString.Create(hiddenInputTag.ToString());
        }
    }
}