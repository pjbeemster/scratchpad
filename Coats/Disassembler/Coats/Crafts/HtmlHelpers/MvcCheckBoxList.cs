namespace Coats.Crafts.HtmlHelpers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Web.Mvc;

    internal static class MvcCheckBoxList
    {
        internal static string EmptyModelMessage = "";
        internal static string EmptyNameMessage = "Name of the CheckBoxList cannot be null or empty";
        internal static string NoDataMessage = "";

        static MvcCheckBoxList()
        {
            linked_label_counter = 0;
            htmlwrap_rowbreak_counter = 0;
        }

        internal static MvcHtmlString CheckBoxList(HtmlHelper htmlHelper, string listName, List<SelectListItem> dataList, object htmlAttributes, HtmlListInfo wrapInfo, string[] disabledValues, Position position = 0)
        {
            if ((dataList == null) || (dataList.Count == 0))
            {
                return MvcHtmlString.Create(NoDataMessage);
            }
            if (string.IsNullOrEmpty(listName))
            {
                throw new ArgumentException(EmptyNameMessage, "listName");
            }
            int count = dataList.Count;
            htmlWrapperInfo htmlWrapper = createHtmlWrapper(wrapInfo, count, position);
            StringBuilder sb = new StringBuilder();
            sb.Append(htmlWrapper.wrap_open);
            htmlwrap_rowbreak_counter = 0;
            IEnumerable<string> selectedValues = from s in dataList
                where s.Selected
                select s.Value;
            foreach (SelectListItem item in dataList)
            {
                sb = createCheckBoxListElement(sb, htmlHelper, null, htmlWrapper, htmlAttributes, selectedValues, disabledValues, listName, item.Value, item.Text);
            }
            sb.Append(htmlWrapper.wrap_close);
            return MvcHtmlString.Create(sb.ToString());
        }

        internal static MvcHtmlString CheckBoxList_ModelBased<TModel, TItem, TValue, TKey>(HtmlHelper<TModel> htmlHelper, ModelMetadata modelMetadata, string listName, Expression<Func<TModel, IEnumerable<TItem>>> sourceDataExpr, Expression<Func<TItem, TValue>> valueExpr, Expression<Func<TItem, TKey>> textToDisplayExpr, Expression<Func<TItem, object>> htmlAttributesExpr, Expression<Func<TModel, IEnumerable<TItem>>> selectedValuesExpr, object htmlAttributes, HtmlListInfo wrapInfo, string[] disabledValues, Position position = 0)
        {
            if ((sourceDataExpr == null) || (sourceDataExpr.Body.ToString() == "null"))
            {
                return MvcHtmlString.Create(NoDataMessage);
            }
            if (htmlHelper.ViewData.Model == null)
            {
                throw new NoNullAllowedException(EmptyModelMessage);
            }
            if (string.IsNullOrEmpty(listName))
            {
                throw new ArgumentException(EmptyNameMessage, "listName");
            }
            TModel model = htmlHelper.ViewData.Model;
            List<TItem> source = sourceDataExpr.Compile()(model).ToList<TItem>();
            Func<TItem, TValue> valueFunc = valueExpr.Compile();
            Func<TItem, TKey> func = textToDisplayExpr.Compile();
            List<TItem> list2 = new List<TItem>();
            if (selectedValuesExpr != null)
            {
                IEnumerable<TItem> enumerable = selectedValuesExpr.Compile()(model);
                if (enumerable != null)
                {
                    list2 = enumerable.ToList<TItem>();
                }
            }
            List<string> selectedValues = (from s in list2 select valueFunc(s).ToString()).ToList<string>();
            if (!source.Any<TItem>())
            {
                return MvcHtmlString.Create(NoDataMessage);
            }
            System.Func<TItem, object, object> func2 = (item, baseAttributes) => baseAttributes;
            if (htmlAttributesExpr != null)
            {
                Func<TItem, object> valueHtmlAttributesFunc = htmlAttributesExpr.Compile();
                func2 = delegate (TItem item, object baseAttributes) {
                    IDictionary<string, object> first = baseAttributes.toDictionary();
                    IDictionary<string, object> second = valueHtmlAttributesFunc(item).toDictionary();
                    ExpandoObject obj2 = new ExpandoObject();
                    IDictionary<string, object> dictionary3 = obj2;
                    foreach (KeyValuePair<string, object> pair in first.Concat<KeyValuePair<string, object>>(second))
                    {
                        dictionary3[pair.Key] = pair.Value;
                    }
                    return obj2;
                };
            }
            int count = source.Count;
            htmlWrapperInfo htmlWrapper = createHtmlWrapper(wrapInfo, count, position);
            StringBuilder sb = new StringBuilder();
            sb.Append(htmlWrapper.wrap_open);
            htmlwrap_rowbreak_counter = 0;
            foreach (TItem local2 in source)
            {
                string itemValue = valueFunc(local2).ToString();
                string itemText = func(local2).ToString();
                sb = createCheckBoxListElement(sb, htmlHelper, modelMetadata, htmlWrapper, func2(local2, htmlAttributes), selectedValues, disabledValues, listName, itemValue, itemText);
            }
            sb.Append(htmlWrapper.wrap_close);
            return MvcHtmlString.Create(sb.ToString());
        }

        private static StringBuilder createCheckBoxListElement(StringBuilder sb, HtmlHelper htmlHelper, ModelMetadata modelMetadata, htmlWrapperInfo htmlWrapper, object htmlAttributesForCheckBox, IEnumerable<string> selectedValues, IEnumerable<string> disabledValues, string name, string itemValue, string itemText)
        {
            ModelState state;
            Func<string, bool> predicate = null;
            string fullHtmlFieldName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            TagBuilder builder = new TagBuilder("input");
            if (selectedValues.Any<string>(x => x == itemValue))
            {
                builder.MergeAttribute("checked", "checked");
            }
            builder.MergeAttributes<string, object>(htmlAttributesForCheckBox.toDictionary());
            builder.MergeAttribute("type", "checkbox");
            builder.MergeAttribute("value", itemValue);
            builder.MergeAttribute("name", fullHtmlFieldName);
            int num1 = linked_label_counter;
            linked_label_counter = num1 + 1;
            string str2 = name + num1;
            builder.GenerateId(str2);
            TagBuilder builder2 = new TagBuilder("label");
            builder2.MergeAttribute("for", str2.Replace(".", "_"));
            builder2.MergeAttributes<string, object>(htmlAttributesForCheckBox.toDictionary());
            builder2.InnerHtml = itemText;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullHtmlFieldName, out state) && (state.Errors.Count > 0))
            {
                builder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
            }
            builder.MergeAttributes<string, object>(htmlHelper.GetUnobtrusiveValidationAttributes(name, modelMetadata));
            sb.Append((htmlWrapper.wrap_element != htmlElementTag.None) ? ("<" + htmlWrapper.wrap_element + ">") : "");
            if ((disabledValues != null) && disabledValues.ToList<string>().Any<string>(x => (x == itemValue)))
            {
                builder.MergeAttribute("disabled", "disabled");
                if (predicate == null)
                {
                    predicate = x => x == itemValue;
                }
                if (selectedValues.Any<string>(predicate))
                {
                    TagBuilder builder3 = new TagBuilder("input");
                    builder3.MergeAttribute("type", "hidden");
                    builder3.MergeAttribute("value", itemValue);
                    builder3.MergeAttribute("name", name);
                    sb.Append(builder3.ToString(TagRenderMode.Normal));
                }
            }
            sb.Append(builder.ToString(TagRenderMode.Normal));
            sb.Append(builder2.ToString(TagRenderMode.Normal));
            sb.Append((htmlWrapper.wrap_element != htmlElementTag.None) ? ("</" + htmlWrapper.wrap_element + ">") : "");
            sb.Append(htmlWrapper.append_to_element);
            htmlwrap_rowbreak_counter++;
            if (htmlwrap_rowbreak_counter == htmlWrapper.separator_max_counter)
            {
                sb.Append(htmlWrapper.wrap_rowbreak);
                htmlwrap_rowbreak_counter = 0;
            }
            return sb;
        }

        private static htmlWrapperInfo createHtmlWrapper(HtmlListInfo wrapInfo, int numberOfItems, Position position)
        {
            htmlWrapperInfo info = new htmlWrapperInfo();
            if (wrapInfo != null)
            {
                htmlElementTag tr;
                TagBuilder builder;
                switch (wrapInfo.htmlTag)
                {
                    case HtmlTag.ul:
                        builder = new TagBuilder(htmlElementTag.ul.ToString());
                        builder.MergeAttributes<string, object>(wrapInfo.htmlAttributes.toDictionary());
                        builder.MergeAttribute("cellspacing", "0");
                        info.wrap_element = htmlElementTag.li;
                        info.wrap_open = builder.ToString(TagRenderMode.StartTag);
                        info.wrap_close = builder.ToString(TagRenderMode.EndTag);
                        return info;

                    case HtmlTag.table:
                        if (wrapInfo.Columns <= 0)
                        {
                            wrapInfo.Columns = 1;
                        }
                        info.separator_max_counter = wrapInfo.Columns;
                        builder = new TagBuilder(htmlElementTag.table.ToString());
                        builder.MergeAttributes<string, object>(wrapInfo.htmlAttributes.toDictionary());
                        builder.MergeAttribute("cellspacing", "0");
                        tr = htmlElementTag.tr;
                        info.wrap_element = htmlElementTag.td;
                        info.wrap_open = string.Concat(new object[] { builder.ToString(TagRenderMode.StartTag), "<", tr, ">" });
                        info.wrap_rowbreak = string.Concat(new object[] { "</", tr, "><", tr, ">" });
                        info.wrap_close = string.Concat(new object[] { "</", tr, ">", builder.ToString(TagRenderMode.EndTag) });
                        return info;

                    case HtmlTag.vertical_columns:
                    {
                        object obj2;
                        if (wrapInfo.Columns <= 0)
                        {
                            wrapInfo.Columns = 1;
                        }
                        int num = Convert.ToInt32(Math.Ceiling((decimal) (Convert.ToDecimal(numberOfItems) / Convert.ToDecimal(wrapInfo.Columns))));
                        if ((numberOfItems <= 4) && ((numberOfItems <= wrapInfo.Columns) || ((numberOfItems - wrapInfo.Columns) == 1)))
                        {
                            num = numberOfItems;
                        }
                        info.separator_max_counter = num;
                        tr = htmlElementTag.div;
                        builder = new TagBuilder(tr.ToString());
                        IDictionary<string, object> attributes = wrapInfo.htmlAttributes.toDictionary();
                        string str = "float:left; margin-right:30px; line-height:25px;";
                        attributes.TryGetValue("style", out obj2);
                        if (obj2 != null)
                        {
                            builder.MergeAttribute("style", str + " " + obj2);
                        }
                        else
                        {
                            builder.MergeAttribute("style", str);
                        }
                        attributes.Remove("style");
                        builder.MergeAttributes<string, object>(attributes);
                        info.wrap_open = builder.ToString(TagRenderMode.StartTag);
                        info.wrap_rowbreak = string.Concat(new object[] { "</", tr, "> ", builder.ToString(TagRenderMode.StartTag) });
                        info.wrap_close = builder.ToString(TagRenderMode.EndTag) + " <div style=\"clear:both;\"></div>";
                        info.append_to_element = "<br/>";
                        return info;
                    }
                }
                return info;
            }
            if (position == Position.Horizontal)
            {
                info.append_to_element = " &nbsp; ";
            }
            if (position == Position.Vertical)
            {
                info.append_to_element = "<br/>";
            }
            return info;
        }

        internal static IDictionary<string, object> toDictionary(this object _object)
        {
            if (_object == null)
            {
                return new Dictionary<string, object>();
            }
            if (_object is IDictionary<string, object>)
            {
                return (IDictionary<string, object>) _object;
            }
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(_object);
            Dictionary<string, object> dictionary = new Dictionary<string, object>(properties.Count);
            foreach (PropertyDescriptor descriptor in properties)
            {
                string key = descriptor.Name.Replace("_", "-");
                object obj2 = descriptor.GetValue(_object);
                dictionary.Add(key, obj2 ?? "");
            }
            return dictionary;
        }

        internal static string toProperty<TModel, TItem>(this Expression<Func<TModel, TItem>> propertyExpression)
        {
            return ExpressionHelper.GetExpressionText(propertyExpression);
        }

        private static int htmlwrap_rowbreak_counter
        {
            [CompilerGenerated]
            get
            {
                return <htmlwrap_rowbreak_counter>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <htmlwrap_rowbreak_counter>k__BackingField = value;
            }
        }

        private static int linked_label_counter
        {
            [CompilerGenerated]
            get
            {
                return <linked_label_counter>k__BackingField;
            }
            [CompilerGenerated]
            set
            {
                <linked_label_counter>k__BackingField = value;
            }
        }

        internal enum htmlElementTag
        {
            None,
            tr,
            td,
            li,
            div,
            table,
            ul
        }

        internal class htmlWrapperInfo
        {
            public string append_to_element = string.Empty;
            public int separator_max_counter;
            public string wrap_close = string.Empty;
            public MvcCheckBoxList.htmlElementTag wrap_element = MvcCheckBoxList.htmlElementTag.None;
            public string wrap_open = string.Empty;
            public string wrap_rowbreak = string.Empty;
        }
    }
}

