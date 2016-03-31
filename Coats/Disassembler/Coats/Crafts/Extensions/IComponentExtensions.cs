namespace Coats.Crafts.Extensions
{
    using DD4T.ContentModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class IComponentExtensions
    {
        public static string DataFilterClasses(this IComponent component, string key)
        {
            StringBuilder builder = new StringBuilder();
            List<string> list = new List<string>();
            if (component.MetadataFields.ContainsKey("facets"))
            {
                IField field = component.MetadataFields["facets"];
                try
                {
                    Func<IKeyword, bool> predicate = null;
                    IComponent component2 = field.LinkedComponentValues.SingleOrDefault<IComponent>(l => l.Schema.RootElementName == "Coats.Crafts.Schemas.Tagging");
                    string qualifiedKey = @"\" + key + @"\";
                    foreach (ICategory category in from c in component2.Categories
                        where c.Keywords.Count<IKeyword>(k => k.Path.StartsWith(qualifiedKey, StringComparison.InvariantCultureIgnoreCase)) > 0
                        select c)
                    {
                        if (predicate == null)
                        {
                            predicate = k => k.Path.StartsWith(qualifiedKey, StringComparison.InvariantCultureIgnoreCase);
                        }
                        foreach (IKeyword keyword in category.Keywords.Where<IKeyword>(predicate))
                        {
                            string path = keyword.Path;
                            while (path.StartsWith(@"\"))
                            {
                                path = path.Remove(0, 1);
                            }
                            try
                            {
                                string str2 = string.Empty;
                                foreach (string str3 in Regex.Split(path, @"\\"))
                                {
                                    Regex regex = new Regex("[^a-z0-9_]");
                                    if (string.IsNullOrEmpty(str2))
                                    {
                                        str2 = component.Publication.Id.Substring(6, 2) + "_" + regex.Replace(str3.ToLower(), "_");
                                    }
                                    else
                                    {
                                        string item = str2 + "__dir__" + regex.Replace(str3.ToLower(), "_");
                                        if (!list.Contains(item))
                                        {
                                            list.Add(item);
                                        }
                                        str2 = item;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            foreach (string str6 in list.ToArray())
            {
                builder.AppendFormat("{0}{1}", (builder.Length > 0) ? " " : "", str6);
            }
            return builder.ToString();
        }

        public static void Populate<T>(this IComponent component, T dataSource)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            PropertyInfo[] infoArray2 = component.GetType().GetProperties();
            Func<PropertyInfo, bool> predicate = null;
            foreach (PropertyInfo target in infoArray2)
            {
                try
                {
                    if (predicate == null)
                    {
                        predicate = s => s.Name == target.Name;
                    }
                    PropertyInfo info = properties.SingleOrDefault<PropertyInfo>(predicate);
                    target.SetValue(component, info.GetValue(dataSource, null), null);
                }
                catch (Exception)
                {
                }
            }
        }

        public static List<Component> ToComponentList(this IList<IComponent> il)
        {
            List<Component> list = new List<Component>();
            foreach (IComponent component in il)
            {
                Component component2 = new Component();
                component2.Populate<IComponent>(component);
                list.Add(component2);
            }
            return list;
        }
    }
}

