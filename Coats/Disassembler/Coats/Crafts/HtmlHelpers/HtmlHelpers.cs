namespace Coats.Crafts.HtmlHelpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;

    public static class HtmlHelpers
    {
        public static MvcHtmlString DropDownGropList(this HtmlHelper htmlHelper, string name)
        {
            return DropDownListHelper(htmlHelper, name, null, null, null);
        }

        public static MvcHtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList)
        {
            return DropDownListHelper(htmlHelper, name, selectList, null, null);
        }

        public static MvcHtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, string optionLabel)
        {
            return DropDownListHelper(htmlHelper, name, null, optionLabel, null);
        }

        public static MvcHtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            return DropDownListHelper(htmlHelper, name, selectList, null, htmlAttributes);
        }

        public static MvcHtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList, object htmlAttributes)
        {
            return DropDownListHelper(htmlHelper, name, selectList, null, new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList, string optionLabel)
        {
            return DropDownListHelper(htmlHelper, name, selectList, optionLabel, null);
        }

        public static MvcHtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            return DropDownListHelper(htmlHelper, name, selectList, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString DropDownGropList(this HtmlHelper htmlHelper, string name, IEnumerable<GroupedSelectListItem> selectList, string optionLabel, object htmlAttributes)
        {
            return DropDownListHelper(htmlHelper, name, selectList, optionLabel, new RouteValueDictionary(htmlAttributes));
        }

        public static MvcHtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList)
        {
            return htmlHelper.DropDownGroupListFor<TModel, TProperty>(expression, selectList, null, ((IDictionary<string, object>) null));
        }

        public static MvcHtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.DropDownGroupListFor<TModel, TProperty>(expression, selectList, null, htmlAttributes);
        }

        public static MvcHtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList, object htmlAttributes)
        {
            return htmlHelper.DropDownGroupListFor<TModel, TProperty>(expression, selectList, null, ((IDictionary<string, object>) new RouteValueDictionary(htmlAttributes)));
        }

        public static MvcHtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList, string optionLabel)
        {
            return htmlHelper.DropDownGroupListFor<TModel, TProperty>(expression, selectList, optionLabel, ((IDictionary<string, object>) null));
        }

        public static MvcHtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            return DropDownListHelper(htmlHelper, ExpressionHelper.GetExpressionText(expression), selectList, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString DropDownGroupListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<GroupedSelectListItem> selectList, string optionLabel, object htmlAttributes)
        {
            return htmlHelper.DropDownGroupListFor<TModel, TProperty>(expression, selectList, optionLabel, ((IDictionary<string, object>) new RouteValueDictionary(htmlAttributes)));
        }

        private static MvcHtmlString DropDownListHelper(HtmlHelper htmlHelper, string expression, IEnumerable<GroupedSelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.SelectInternal(optionLabel, expression, selectList, false, htmlAttributes);
        }

        internal static object GetModelStateValue(this HtmlHelper helper, string key, Type destinationType)
        {
            ModelState state;
            if (helper.ViewData.ModelState.TryGetValue(key, out state) && (state.Value != null))
            {
                return state.Value.ConvertTo(destinationType, null);
            }
            return null;
        }

        private static IEnumerable<GroupedSelectListItem> GetSelectData(this HtmlHelper htmlHelper, string name)
        {
            object obj2 = null;
            IEnumerable<GroupedSelectListItem> enumerable = null;
            if (htmlHelper.ViewData != null)
            {
                obj2 = htmlHelper.ViewData.Eval(name);
            }
            if (obj2 != null)
            {
                enumerable = obj2 as IEnumerable<GroupedSelectListItem>;
            }
            return enumerable;
        }

        internal static string ListItemToOption(GroupedSelectListItem item)
        {
            TagBuilder builder = new TagBuilder("option") {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };
            if (item.Value != null)
            {
                builder.Attributes["value"] = item.Value;
            }
            if (item.Selected)
            {
                builder.Attributes["selected"] = "selected";
            }
            return builder.ToString(TagRenderMode.Normal);
        }

        private static MvcHtmlString SelectInternal(this HtmlHelper htmlHelper, string optionLabel, string name, IEnumerable<GroupedSelectListItem> selectList, bool allowMultiple, IDictionary<string, object> htmlAttributes)
        {
            try
            {
                name = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
                if (!string.IsNullOrEmpty(name))
                {
                    ModelState state;
                    bool flag = false;
                    if (selectList == null)
                    {
                        selectList = htmlHelper.GetSelectData(name);
                        flag = true;
                    }
                    object obj2 = allowMultiple ? htmlHelper.GetModelStateValue(name, typeof(string[])) : htmlHelper.GetModelStateValue(name, typeof(string));
                    if (!flag && (obj2 == null))
                    {
                        obj2 = htmlHelper.ViewData.Eval(name);
                    }
                    if (obj2 != null)
                    {
                        IEnumerable source = allowMultiple ? (obj2 as IEnumerable) : ((IEnumerable) new object[] { obj2 });
                        HashSet<string> set = new HashSet<string>(from value in source.Cast<object>() select Convert.ToString(value, CultureInfo.CurrentCulture), StringComparer.OrdinalIgnoreCase);
                        List<GroupedSelectListItem> list = new List<GroupedSelectListItem>();
                        if (selectList != null)
                        {
                            foreach (GroupedSelectListItem item in selectList)
                            {
                                item.Selected = (item.Value != null) ? set.Contains(item.Value) : set.Contains(item.Text);
                                list.Add(item);
                            }
                            selectList = list;
                        }
                    }
                    StringBuilder builder = new StringBuilder();
                    if (optionLabel != null)
                    {
                        GroupedSelectListItem item2 = new GroupedSelectListItem {
                            Text = optionLabel,
                            Value = string.Empty,
                            Selected = false
                        };
                        builder.AppendLine(ListItemToOption(item2));
                    }
                    using (IEnumerator<IGrouping<string, GroupedSelectListItem>> enumerator2 = (from i in selectList group i by i.GroupKey).GetEnumerator())
                    {
                        Func<GroupedSelectListItem, bool> predicate = null;
                        IGrouping<string, GroupedSelectListItem> group;
                        while (enumerator2.MoveNext())
                        {
                            group = enumerator2.Current;
                            if (predicate == null)
                            {
                                predicate = i => i.GroupKey == group.Key;
                            }
                            string str = (from it in selectList.Where<GroupedSelectListItem>(predicate) select it.GroupName).FirstOrDefault<string>();
                            if (group.Count<GroupedSelectListItem>() > 1)
                            {
                                builder.AppendLine(string.Format("<optgroup label=\"{0}\" value=\"{1}\">", str, group.Key));
                                foreach (GroupedSelectListItem item in group)
                                {
                                    builder.AppendLine(ListItemToOption(item));
                                }
                                builder.AppendLine("</optgroup>");
                            }
                            else
                            {
                                foreach (GroupedSelectListItem item in group)
                                {
                                    builder.AppendLine(ListItemToOption(item));
                                }
                            }
                        }
                    }
                    TagBuilder builder2 = new TagBuilder("select") {
                        InnerHtml = builder.ToString()
                    };
                    builder2.MergeAttributes<string, object>(htmlAttributes);
                    builder2.MergeAttribute("name", name, true);
                    builder2.GenerateId(name);
                    if (allowMultiple)
                    {
                        builder2.MergeAttribute("multiple", "multiple");
                    }
                    if (htmlHelper.ViewData.ModelState.TryGetValue(name, out state) && (state.Errors.Count > 0))
                    {
                        builder2.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                    }
                    return MvcHtmlString.Create(builder2.ToString());
                }
            }
            catch (Exception)
            {
            }
            return MvcHtmlString.Create("");
        }
    }
}

