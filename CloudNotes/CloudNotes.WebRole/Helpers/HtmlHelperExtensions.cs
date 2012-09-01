using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Routing;

namespace CloudNotes.WebRole.Helpers
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Helper to build a div to show validation errors or some other type of error with the style provided by the Bootstrapper.
        /// </summary>
        /// <param name="htmlHelper">HtmlHelper to extend</param>
        /// <param name="validationErrors">List of validation errors to show in the div</param>
        /// <param name="htmlAttributes">Html attributes to apply to the div</param>
        /// <returns></returns>
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

        /// <summary>
        /// Builds a div to show alerts wiht the style provided by Bootstrapper.
        /// </summary>
        /// <param name="htmlHelper">HtmlHelper to extend</param>
        /// <param name="alertMessage">Alert message to show in the div</param>
        /// <param name="htmlAttributes">Html attributes to apply to the div</param>
        /// <returns></returns>
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

        /// <summary>
        /// Builds a fieldset html element.
        /// </summary>
        /// <param name="htmlHelper">HtmlHelper to extend</param>
        /// <param name="legend">Field set legend</param>
        /// <param name="htmlAttributes">Html attributes to apply to the fieldset</param>
        /// <returns></returns>
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

        /// <summary>
        /// Builds a html label.
        /// </summary>
        /// <param name="htmlHelper">HtmlHelper to extend</param>
        /// <param name="label">Label text</param>
        /// <param name="for">The html element that the label is related</param>
        /// <param name="htmlAttributes">Html attributes to apply to the label</param>
        /// <returns></returns>
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

        /// <summary>
        /// Builds a html button.
        /// </summary>
        /// <param name="htmlHelper">HtmlHelper to extend</param>
        /// <param name="type">button type</param>
        /// <param name="text">button text</param>
        /// <param name="htmlAttributes">Html attributes to apply to the button</param>
        /// <returns></returns>
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

        /// <summary>
        /// Builds a hidden input html element
        /// </summary>
        /// <param name="htmlHelper">HtmlHelper to extend</param>
        /// <param name="id">the hidden input id</param>
        /// <param name="value">the hidden input value</param>
        /// <returns></returns>
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