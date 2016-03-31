using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DD4T.ContentModel;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Coats.Crafts.Extensions
{
    public static class IComponentExtensions
    {
        public static void Populate<T>(this IComponent component, T dataSource)
        {
            PropertyInfo[] sourceProperties = typeof(T).GetProperties();
            PropertyInfo[] targetProperties = component.GetType().GetProperties();

            foreach (PropertyInfo target in targetProperties)
            {
                try
                {
                    var source = sourceProperties.SingleOrDefault(s => s.Name == target.Name);
                    target.SetValue(component, source.GetValue(dataSource, null), null);
                }
                catch (Exception) { /* Silent catch */ }
            }

        }

        public static List<Component> ToComponentList(this IList<IComponent> il)
        {
            List<Component> components = new List<Component>();
            foreach (IComponent ic in il)
            {
                Component comp = new Component();
                comp.Populate(ic);
                components.Add(comp);
            }
            return components;
        }

        public static string DataFilterClasses(this IComponent component, string key)
        {
            #region Previous Logic (reference only)
            //// Trying to locate something like component.MetadataFields["facets"].LinkedComponentValues[0].Fields["techniques"].Keywords
            //StringBuilder dataFilter = new StringBuilder();
            //if (component.MetadataFields.ContainsKey("facets"))
            //{
            //    var mf = component.MetadataFields["facets"];
            //    try
            //    {
            //        // Will most probably only be one LinkedComponentValue, but using linq here to make sure
            //        var lcv = mf.LinkedComponentValues.SingleOrDefault(l => l.Fields.ContainsKey(key));

            //        foreach (var field in lcv.Fields[key].Values)
            //        {

            //            // Clean invalid characters
            //            var cleanedField = field
            //                                    .ToLower()
            //                                    .Replace(" ", "_")
            //                                    .Replace("(", String.Empty)
            //                                    .Replace(")", String.Empty)
            //                                    .Replace("&", String.Empty)
            //                                    .Replace("/", String.Empty)
            //                                    .Replace("\\", String.Empty)
            //                                    .Replace("__", "_");

            //            char[] charsToTrim = { ' ', '_' };

            //            var tidiedClean = cleanedField.Trim(charsToTrim);

            //            // List should be something like "knitting sewing";
            //            dataFilter.AppendFormat("{0}{1}", (dataFilter.Length > 0 ? " " : ""), tidiedClean);
            //        }
            //    }
            //    catch (Exception) { /* Silent catch */ }
            //}
            //return dataFilter.ToString();
            #endregion

            // Trying to locate something like component.MetadataFields["facets"].LinkedComponentValues[0].Categories[n].Keywords[n].Path
            StringBuilder dataFilter = new StringBuilder();
            List<string> keyItems = new List<string>();

            // First, check if there is a metadata fieldset containing facets
            if (component.MetadataFields.ContainsKey("facets"))
            {
                // Now get the actual facets metadata fieldset 
                var mf = component.MetadataFields["facets"];
                try
                {
                    // *** CHECK THIS IS ACTUALLY THE CASE ***
                    // Will most probably only be one LinkedComponentValue, but using linq here to make sure.
                    var lcv = mf.LinkedComponentValues.SingleOrDefault(l => l.Schema.RootElementName == "Coats.Crafts.Schemas.Tagging");

                    // Categories contain every faceted content tagging item, from end uses to techniques to skill levels, etc.
                    // We are only interested in the categories containing a path that starts with the key (e.g. "\\techniques\\").
                    string qualifiedKey = "\\" + key + "\\";
                    foreach (var cat in lcv.Categories.Where(c => c.Keywords.Count(k => k.Path.StartsWith(qualifiedKey, StringComparison.InvariantCultureIgnoreCase)) > 0))
                    {
                        foreach (var keyword in cat.Keywords.Where(k => k.Path.StartsWith(qualifiedKey, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            /*
                            The Category.Path will be in a format similar to:
                            "\\Techniques\\Crochet\\Granny Squares"
                            To future proof, we need to create a path key for each path segment:
                            "techniques__dir__crochet"
                            "techniques__dir__crochet__dir__granny_squares"
                            To get "techniques__dir__crochet techniques__dir__crochet__dir__granny_squares"
                            */

                            // Strip starting backslashes
                            string path = keyword.Path;
                            while(path.StartsWith("\\"))
                            {
                                path = path.Remove(0, 1);
                            }

                            try
                            {
                                // Build each segment as a path key
                                string prev = string.Empty;
                                foreach (string p in Regex.Split(path, "\\\\"))
                                {
                                    // Regex to replace anything other than an alpha numeric and underscore with an underscore
                                    Regex rgx = new Regex("[^a-z0-9_]");

                                    if (string.IsNullOrEmpty(prev)) 
                                    {
                                        // Don't build a path key if it is the first segment (e.g. "techniques").
                                        // Just store it in the previous string for the next itteration.

                                        // We also need to include the publication Id in the path
                                        string publicationId = component.Publication.Id.Substring(6, 2);

                                        prev = publicationId + "_" + rgx.Replace(p.ToLower(), "_");
                                        continue; 
                                    }

                                    // Build the current path key by appending to the previous path key
                                    string cur = prev + "__dir__" + rgx.Replace(p.ToLower(), "_");

                                    // Check if we have this in our list already (de-dup)
                                    if (!keyItems.Contains(cur))
                                    {
                                        keyItems.Add(cur);
                                    }

                                    // Finally, set the previous string to the current for the next itteration.
                                    prev = cur;
                                }

                            }
                            catch (Exception) { /* Silent catch */ }
                        }
                    }

                }
                catch (Exception) { /* Silent catch */ }
            }

            // Finally, append the list of path keys together using a space as the delimiter.
            // e.g. "techniques__dir__crochet techniques__dir__crochet__dir__granny_squares"
            foreach (string item in keyItems.ToArray())
            {
                dataFilter.AppendFormat("{0}{1}", (dataFilter.Length > 0 ? " " : ""), item);
            }

            return dataFilter.ToString();
        }
    }
}