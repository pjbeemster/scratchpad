namespace Coats.Crafts.HtmlHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Web.Mvc;

    public static class CheckBoxHelper
    {
        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string listName, List<SelectListItem> dataList, HtmlListInfo wrapInfo)
        {
            return htmlHelper.CheckBoxList(listName, dataList, null, wrapInfo, null, Position.Horizontal);
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string listName, List<SelectListItem> dataList, Position position = 0)
        {
            return htmlHelper.CheckBoxList(listName, dataList, null, null, null, position);
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string listName, List<SelectListItem> dataList, HtmlListInfo wrapInfo, string[] disabledValues)
        {
            return htmlHelper.CheckBoxList(listName, dataList, null, wrapInfo, disabledValues, Position.Horizontal);
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string listName, List<SelectListItem> dataList, object htmlAttributes, Position position = 0)
        {
            return htmlHelper.CheckBoxList(listName, dataList, htmlAttributes, null, null, position);
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string listName, List<SelectListItem> dataList, object htmlAttributes, string[] disabledValues, Position position = 0)
        {
            return htmlHelper.CheckBoxList(listName, dataList, htmlAttributes, null, disabledValues, position);
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper htmlHelper, string listName, List<SelectListItem> dataList, object htmlAttributes, HtmlListInfo wrapInfo, string[] disabledValues, Position position = 0)
        {
            return MvcCheckBoxList.CheckBoxList(htmlHelper, listName, dataList, htmlAttributes, wrapInfo, disabledValues, position);
        }
    }
}

