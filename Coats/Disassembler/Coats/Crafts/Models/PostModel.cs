namespace Coats.Crafts.Models
{
    using Coats.Crafts.Configuration;
    using Coats.Crafts.ControllerHelpers;
    using Facebook;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class PostModel
    {
        private string GetAccessToken()
        {
            string facebookAppId = WebConfiguration.Current.FacebookAppId;
            string facebookAppSecret = WebConfiguration.Current.FacebookAppSecret;
            FacebookClient client = new FacebookClient();
            dynamic obj2 = client.Get("oauth/access_token", new { 
                client_id = facebookAppId,
                client_secret = facebookAppSecret,
                grant_type = "client_credentials"
            });
            client.AccessToken = (string) obj2.access_token;
            return client.AccessToken;
        }

        public IQueryable<PostModel> GetAll(string facebookID, string orientation)
        {
            List<PostModel> list = new List<PostModel>();
            string str = string.Empty;
            string accessToken = string.Empty;
            str = new oAuthFacebook().AuthorizationLinkGet();
            accessToken = this.GetAccessToken();
            int count = Convert.ToInt32(WebConfiguration.Current.FacebookNumberToReturn);
            count = 3;
            Dictionary<int, object> posts = new Dictionary<int, object>();
            posts = this.GetPosts(accessToken, facebookID);
            foreach (dynamic obj2 in posts)
            {
                try
                {
                    dynamic obj3 = obj2.Value;
                    PostModel item = new PostModel {
                        Orientation = orientation,
                        FacebookID = facebookID
                    };
                    try
                    {
                        item.Caption = (string) obj3.caption;
                    }
                    catch
                    {
                        item.Caption = string.Empty;
                    }
                    try
                    {
                        item.Picture = (string) obj3.picture;
                    }
                    catch
                    {
                        item.Picture = string.Empty;
                    }
                    try
                    {
                        item.Story = (string) obj3.story;
                    }
                    catch
                    {
                        item.Story = string.Empty;
                    }
                    try
                    {
                        item.Message = (string) obj3.message;
                    }
                    catch
                    {
                        item.Message = string.Empty;
                    }
                    try
                    {
                        item.Link = (string) obj3.link;
                    }
                    catch
                    {
                        item.Link = string.Empty;
                    }
                    try
                    {
                        item.Icon = (string) obj3.icon;
                    }
                    catch
                    {
                        item.Icon = string.Empty;
                    }
                    try
                    {
                        item.FCTime = (DateTime) DateTime.Parse(obj3.created_time);
                    }
                    catch
                    {
                        item.FCTime = DateTime.Now;
                    }
                    try
                    {
                        item.FUTime = (DateTime) DateTime.Parse(obj3.updated_time);
                    }
                    catch
                    {
                        item.FUTime = DateTime.Now;
                    }
                    try
                    {
                        item.FName = (string) obj3.name;
                    }
                    catch
                    {
                        item.FName = string.Empty;
                    }
                    try
                    {
                        item.Type = (string) obj3.type;
                    }
                    catch
                    {
                        item.Type = string.Empty;
                    }
                    try
                    {
                        item.StatusType = (string) obj3.status_type;
                    }
                    catch
                    {
                        item.StatusType = string.Empty;
                    }
                    if (!string.IsNullOrEmpty(item.Link))
                    {
                        list.Add(item);
                    }
                }
                catch (Exception)
                {
                }
            }
            List<string> typeList = WebConfiguration.Current.FaceBookType.Split(new char[] { ',' }).ToList<string>();
            List<PostModel> source = (from o in list
                where typeList.Contains(o.Type)
                orderby o.FCTime descending
                select o).ToList<PostModel>();
            if (source.Count > count)
            {
                return source.GetRange(0, count).AsQueryable<PostModel>();
            }
            return source.AsQueryable<PostModel>();
        }

        private Dictionary<int, object> GetPosts(string accessToken, string facebookID)
        {
            Dictionary<int, object> dictionary = new Dictionary<int, object>();
            string path = facebookID + "/posts";
            string str2 = facebookID + "?fields=picture";
            try
            {
                int num = 0;
                FacebookClient client = new FacebookClient(accessToken);
                dynamic obj2 = client.Get(str2);
                object obj3 = ((ICollection<object>) obj2.Values).First<object>();
                obj2 = client.Get(path);
                ICollection<object> is2 = (ICollection<object>) obj2.Values;
                foreach (object obj4 in is2)
                {
                    foreach (object obj5 in (IEnumerable) obj4)
                    {
                        try
                        {
                            dictionary.Add(num, (dynamic) obj5);
                            num++;
                        }
                        catch
                        {
                        }
                    }
                }
                return dictionary;
            }
            catch (Exception)
            {
                return dictionary;
            }
        }

        [Display(Name="Caption")]
        public string Caption { get; set; }

        [Display(Name="FacebookID")]
        public string FacebookID { get; set; }

        public string FBId { get; set; }

        public string FBLongLivedToken { get; set; }

        public string FBShortLivedToken { get; set; }

        [Display(Name="CreatedTime")]
        public DateTime FCTime { get; set; }

        [Display(Name="Name")]
        public string FName { get; set; }

        [Display(Name="UpdatedTime")]
        public DateTime FUTime { get; set; }

        [Display(Name="Icon")]
        public string Icon { get; set; }

        [Display(Name="Link")]
        public string Link { get; set; }

        [Display(Name="Message")]
        public string Message { get; set; }

        [Display(Name="Orientation")]
        public string Orientation { get; set; }

        [Display(Name="Picture")]
        public string Picture { get; set; }

        [Display(Name="StatusType")]
        public string StatusType { get; set; }

        [Display(Name="Story")]
        public string Story { get; set; }

        [Display(Name="Type")]
        public string Type { get; set; }
    }
}

