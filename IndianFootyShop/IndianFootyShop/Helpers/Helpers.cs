using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IndianFootyShop
{
    public static class Helpers
    {
        #region ChecboxListFor
        public static MvcHtmlString ChecboxListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> modelSelector, Func<TModel, bool> Expression, string dataTextField, string valueField, int columns, IEnumerable dataSource, IEnumerable selectedValues = null, object htmlAttributes = null, string modelBindingProperty = null)
        {
            string name = ExpressionHelper.GetExpressionText(modelSelector);
            string rootName = modelSelector.Name;
            object value = ModelMetadata.FromLambdaExpression<TModel, TProperty>(modelSelector, htmlHelper.ViewData).Model;
            return OptionList<TModel>(name, Expression, dataTextField, valueField, columns, htmlAttributes, dataSource, selectedValues, rootName, modelBindingProperty);
        }
        #endregion

        #region OptionList
        private static MvcHtmlString OptionList<T>(string Name, Func<T, bool> Expression, string dataTextField, string valueField, int Columns, object htmlAttributes, IEnumerable dataSoureList, IEnumerable selectedValues = null, string RootName = null, string ModelBindingPropertyName = null)
        {

            ModelBindingPropertyName = (string.IsNullOrEmpty(ModelBindingPropertyName) ? string.Empty : ModelBindingPropertyName);
            RouteValueDictionary htmlAttributeDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            string id = string.Empty;
            if (htmlAttributeDictionary["id"] == null)
            {
                id = Name;
            }
            else
            {
                id = htmlAttributeDictionary["id"].ToString();
            }
            TagBuilder optionlist = new TagBuilder("div");
            optionlist.Attributes.Add("style", "width:100%;display:inline-block");
            //optionlist.MergeAttributes<string, object>(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            TagBuilder column = null;
            int columnwidth = 100 / Columns;
            //IEnumerable<SelectListItem> _finalList = null;
            MultiSelectList _finalList = null;
            SelectList _selectedList = new SelectList(selectedValues); ;


            _finalList = new MultiSelectList(dataSoureList, dataTextField, valueField, selectedValues);

            int listCount = _finalList.Count<SelectListItem>();
            int itemsPerColumn = (int)Math.Ceiling(Convert.ToDouble(listCount) / (double)Columns);
            int itemCounterPerColumn = 0;
            int columnCounter = 0;
            string optionName = string.Empty;
            string optionLabel = "  <label style='display:inline-block;' for='{0}' class='vlabel' name='{1}'>{2}</label>";
            string optionInput = "<input style='display:inline-block;' id='{0}' type='hidden' name='{1}' value='{2}' ></input>";
            string tagstring = string.Empty;
            int listCounter = 0;


            foreach (SelectListItem item in _finalList)
            {
                if (itemCounterPerColumn == 0)
                {
                    column = new TagBuilder("div");
                    column.Attributes.Add("style", "width:" + columnwidth.ToString() + "%;float:left;");
                    columnCounter++;
                }
                TagBuilder option = new TagBuilder("input");
                option.Attributes.Add("type", "checkbox".ToString());
                option.MergeAttribute("value", item.Value.ToString());
                option.MergeAttribute("style", "margin:3px;");



                if (item.Selected)
                {
                    option.MergeAttribute("checked", "checked");
                }
                optionName = string.Concat(new object[]
			        {
				        Name,
				        "[",
				        listCounter,
				        "].",
				        ModelBindingPropertyName
			        });

                option.MergeAttribute("name", optionName);
                option.MergeAttribute("id", string.Concat(new object[]
		            {
			            id,
			            ".option[",
			            listCounter,
			            "]"
		            }));
                option.MergeAttributes<string, object>(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
                column.InnerHtml = column.InnerHtml + option.ToString(TagRenderMode.SelfClosing) + string.Format(optionLabel, string.Concat(new object[]
		            {
			            id,
			            ".option[",
			            listCounter,
			            "]"
		            }), string.Concat(new object[] { Name, "[", listCounter, "].", valueField }), item.Text.ToString()) + string.Format(optionInput, string.Concat(new object[]
		            {
			            Name,
			            "value.option[",
			            listCounter,
			            "]"
		            }), string.Concat(new object[] { Name, "[", listCounter, "].", valueField }), item.Text.ToString()) + "<br />";

                itemCounterPerColumn++;
                if (itemCounterPerColumn == itemsPerColumn || listCounter == listCount - 1)
                {
                    optionlist.InnerHtml += column.ToString();
                    itemCounterPerColumn = 0;
                }
                listCounter++;
            }

            tagstring = optionlist.ToString();
            int counter = 0;
            foreach (SelectListItem item in _finalList)
            {
                TagBuilder hiddenIndex = new TagBuilder("input");
                hiddenIndex.MergeAttribute("type", "hidden");
                hiddenIndex.MergeAttribute("name", Name + ".Index");
                hiddenIndex.MergeAttribute("value", counter.ToString());
                counter++;
                tagstring += hiddenIndex.ToString();
            }

            return new MvcHtmlString(tagstring);
        }
        #endregion

        #region LabelFor
        public static MvcHtmlString LabelFor<TModel, TValue>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TValue>> expression, object htmlAttributes, params object[] args)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);

            var parameters = args.Select(x => htmlHelper.Encode(x)).ToArray();

            //labelText ??

            string resolvedLabelText = metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            resolvedLabelText = string.Format(resolvedLabelText, parameters);
            if (String.IsNullOrEmpty(resolvedLabelText))
            {
                return MvcHtmlString.Empty;
            }

            TagBuilder tag = new TagBuilder("label");
            tag.Attributes.Add("for", TagBuilder.CreateSanitizedId(htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName)));
            tag.SetInnerText(resolvedLabelText);
            tag.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), replaceExisting: true);
            return new MvcHtmlString(tag.ToString());
        }
        #endregion

        #region FileInput
        /// <summary>
        /// Renders a File Input Control.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name">The name attribute which is the name of the property to bind to.</param>
        /// <param name="labelText">The text to appear, to open file dialog on click.</param>
        /// <param name="htmlAttributes">The HTML attributes that to apply on File Input Tag inside the control.</param>
        /// <returns></returns>
        public static MvcHtmlString FileInput(this HtmlHelper htmlHelper, string name, string labelText, object htmlAttributes = null)
        {
            TagBuilder divWrapper = new TagBuilder("div");
            divWrapper.Attributes.Add("class", "fileinputHelper");

            TagBuilder inputTextTag = new TagBuilder("input");
            inputTextTag.Attributes.Add("class", "fileInput_fileName");
            inputTextTag.Attributes.Add("type", "text");
            inputTextTag.Attributes.Add("title", "upload file");


            TagBuilder browseLinkTag = new TagBuilder("div");
            browseLinkTag.Attributes.Add("class", "fileInput_OpenFileDialogButton");
            browseLinkTag.SetInnerText(labelText);

            TagBuilder fakeFileDivWrapper = new TagBuilder("div");
            fakeFileDivWrapper.Attributes.Add("class", "fakefile");
            fakeFileDivWrapper.InnerHtml = inputTextTag.ToString() + browseLinkTag.ToString();

            TagBuilder fileTag = new TagBuilder("input");


            fileTag.Attributes.Add("type", "file");
            fileTag.Attributes.Add("class", "fileInput_file");
            fileTag.Attributes.Add("name", name);
            if (htmlAttributes != null)
                fileTag.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), replaceExisting: false);


            divWrapper.InnerHtml = fileTag.ToString() + fakeFileDivWrapper.ToString();

            return new MvcHtmlString(divWrapper.ToString());
        }
        #endregion

    }
}