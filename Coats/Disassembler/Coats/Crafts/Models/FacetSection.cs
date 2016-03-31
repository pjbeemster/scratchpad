namespace Coats.Crafts.Models
{
    using Coats.Crafts.Extensions;
    using Coats.Crafts.FASWebService;
    using com.fredhopper.lang.query;
    using com.fredhopper.lang.query.location;
    using com.fredhopper.lang.query.location.criteria;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;

    public class FacetSection
    {
        public void SetHrefLinks(Query query)
        {
            Location location = new Location(query.getLocation());
            int offset = query.getListStartIndex();
            query.removeListStartIndex();
            int num2 = this.Facets.Count<FacetItem>(f => f.Selected);
            string ticked = HttpContext.Current.Request["fh_ticked"] ?? "";
            foreach (FacetItem item in this.Facets)
            {
                Location location2 = new Location(location);
                MultiValuedCriterion mvc = location2.getCriterion("schematitle") as MultiValuedCriterion;
                if (mvc != null)
                {
                    (from f in this.Facets
                        where !f.Enabled
                        select f).ToList<FacetItem>().ForEach(f => mvc.getGreaterThan().remove(f.Value));
                    if (string.IsNullOrEmpty(ticked))
                    {
                        if (num2 == mvc.getGreaterThan().valueSet().size())
                        {
                            item.Selected = false;
                            mvc.getGreaterThan().clear().add(item.Value);
                        }
                    }
                    else
                    {
                        if (ticked.Split(new char[] { ',' }).Contains<string>(item.Value))
                        {
                            mvc.getGreaterThan().remove(item.Value);
                        }
                        else
                        {
                            mvc.getGreaterThan().add(item.Value);
                        }
                        if (mvc.getGreaterThan().isEmpty())
                        {
                            location2.removeCriteria("schematitle");
                        }
                    }
                    query.setLocation(location2);
                    item.Href = query.ToFhParams();
                    if (string.IsNullOrEmpty(ticked))
                    {
                        item.Href = item.Href + "&fh_ticked=" + item.Value;
                    }
                    else
                    {
                        List<string> items = ticked.Split(new char[] { ',' }).ToList<string>();
                        (from f in this.Facets
                            where !f.Enabled
                            select f).ToList<FacetItem>().ForEach(f => items.Remove(f.Value));
                        if (items.Contains(item.Value))
                        {
                            items.Remove(item.Value);
                        }
                        else
                        {
                            items.Add(item.Value);
                        }
                        item.Href = Regex.Replace(item.Href, @"&fh_ticked=[\w|,]+", "");
                        if (items.Count > 0)
                        {
                            item.Href = item.Href + "&fh_ticked=" + string.Join(",", items);
                        }
                    }
                }
            }
            query.setLocation(location);
            query.setListStartIndex(offset);
            this.Facets.ToList<FacetItem>().ForEach(delegate (FacetItem f) {
                if (f.Enabled)
                {
                    f.Selected = false;
                }
                List<string> list = ticked.Split(new char[] { ',' }).ToList<string>();
                if (f.Enabled && list.Contains(f.Value))
                {
                    f.Selected = true;
                }
            });
        }

        public string ToLocationString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("schematitle>{");
            try
            {
                foreach (FacetItem item in from f in this.Facets
                    where f.Selected
                    select f)
                {
                    builder.AppendFormat("{0};", item.Value);
                }
                builder.Append("}");
                return builder.ToString().Replace(";}", "}");
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public customfield[] CustomFields { get; set; }

        public IList<FacetItem> Facets { get; set; }

        public string on { get; set; }

        public string SectionTitle { get; set; }

        public bool Visible { get; set; }
    }
}

