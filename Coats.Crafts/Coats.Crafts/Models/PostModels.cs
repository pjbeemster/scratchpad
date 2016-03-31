using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Coats.Crafts.Data;
using Coats.Crafts.ControllerHelpers;
using Coats.Crafts.Configuration;
using System.ComponentModel.DataAnnotations;
using Facebook;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;

namespace Coats.Crafts.Models
{
    public class PostModel
    {
        [Display(Name = "Orientation")]
        public string Orientation { get; set; }

        [Display(Name = "Picture")]
        public string Picture { get; set; }

        [Display(Name = "Caption")]
        public string Caption { get; set; }

        [Display(Name = "Story")]
        public string Story { get; set; }

        [Display(Name = "Link")]
        public string Link { get; set; }

        [Display(Name = "Name")]
        public string FName { get; set; }

        [Display(Name = "Icon")]
        public string Icon { get; set; }

        [Display(Name = "CreatedTime")]
        public DateTime FCTime { get; set; }

        [Display(Name = "UpdatedTime")]
        public DateTime FUTime { get; set; }

        [Display(Name = "Message")]
        public string Message { get; set; }

        [Display(Name = "FacebookID")]
        public string FacebookID { get; set; }

		[Display(Name = "Type")]
		public string Type { get; set; }

		[Display(Name = "StatusType")]
		public string StatusType { get; set; }

        // Hidden fields
        public string FBId { get; set; }
        public string FBShortLivedToken { get; set; }
        public string FBLongLivedToken { get; set; }

        public IQueryable<PostModel> GetAll(string facebookID,string orientation)
        {
            List<PostModel> posts= new List<PostModel>();
            // Get Token
            string accessTokenURL = string.Empty;
            string accessToken = string.Empty;
            oAuthFacebook facebookauth = new oAuthFacebook();
            accessTokenURL = facebookauth.AuthorizationLinkGet();

            accessToken = GetAccessToken();
            int numberToReturn = Convert.ToInt32(WebConfiguration.Current.FacebookNumberToReturn);
            numberToReturn = 3;
            // Get Posts
            Dictionary<int, object> postlist = new Dictionary<int, object>();
            postlist = GetPosts(accessToken, facebookID);

			foreach (dynamic post in postlist)
            {
                try
                {
                    dynamic pval = post.Value;
                    PostModel outpost = new PostModel();
                    outpost.Orientation = orientation;
                    outpost.FacebookID = facebookID;
                    
					try
                    {
                        outpost.Caption = pval.caption;
                    }
                    catch
                    {
                        outpost.Caption = string.Empty;
                    }
                    try
                    {
                        outpost.Picture = pval.picture;
                    }
                    catch
                    {
                        outpost.Picture = string.Empty;
                    }
                    try
                    {
                        outpost.Story = pval.story;
                    }
                    catch
                    {
                        outpost.Story = string.Empty;
                    }
                    try
                    {
                        outpost.Message = pval.message;
                    }
                    catch
                    {
                        outpost.Message = string.Empty;
                    }
                    try
                    {
                        outpost.Link = pval.link;
                    }
                    catch
                    {
                        outpost.Link = string.Empty;
                    }
                    try
                    {
                        outpost.Icon = pval.icon;
                    }
                    catch
                    {
                        outpost.Icon = string.Empty;
                    }
                    try
                    {
                        outpost.FCTime = DateTime.Parse(pval.created_time);
                    }
                    catch
                    {
                        outpost.FCTime = DateTime.Now;
                    }
                    try
                    {
                        outpost.FUTime = DateTime.Parse(pval.updated_time);
                    }
                    catch
                    {
                        outpost.FUTime = DateTime.Now;
                    }
                    try
                    {
                        outpost.FName = pval.name;
                    }
                    catch
                    {
                        outpost.FName = string.Empty;
                    }

					try
                    {
                        outpost.Type = pval.type;
                    }
                    catch
                    {
                        outpost.Type = string.Empty;
                    }

					try
					{
						outpost.StatusType = pval.status_type;
					}
					catch
					{
						outpost.StatusType = string.Empty;
					}

                    if (!string.IsNullOrEmpty(outpost.Link))
                    {
                        posts.Add(outpost);
                    }
                }
                catch (Exception ex)
                {
                    // ignore posts without these
                }
            }

			//CD - created two lists to query against for type and status_type
			List<string> typeList = WebConfiguration.Current.FaceBookType.Split(',').ToList();
			

			List<PostModel> SortedList = posts.Where(o => typeList.Contains(o.Type)).OrderByDescending(o => o.FCTime).ToList();
            
			if (SortedList.Count > numberToReturn)
            {
                return SortedList.GetRange(0, numberToReturn).AsQueryable();
            }

            return SortedList.AsQueryable();
        }

        private Dictionary<int, object> GetPosts(string accessToken, string facebookID)
        {
            Dictionary<int, object> outlist = new Dictionary<int, object>();
            string poster = facebookID + "/posts";
            string posterPicture = facebookID + "?fields=picture";
            try
            {
                int tag = 0;
                var client = new FacebookClient(accessToken);

                dynamic me = client.Get(posterPicture);

                ICollection<dynamic> vals = me.Values;

                dynamic thisphoto = vals.First();


                me = client.Get(poster);
				vals = me.Values;
                foreach (dynamic posts in vals)
                {
                    foreach (dynamic post in posts)
                    {
                        try
                        {
                            outlist.Add(tag, post);
                            tag = tag + 1;
                        }
                        catch
                        {
                        }
                    }
                }
                return outlist;
            }
            catch (Exception ex)
            {
                return outlist;
            }
        }
        
		private string GetAccessToken()
        {
            string appId = WebConfiguration.Current.FacebookAppId;
            string appSecret = WebConfiguration.Current.FacebookAppSecret;
            var fb = new FacebookClient();
            dynamic result = fb.Get("oauth/access_token", new
            {
                client_id = appId,
                client_secret = appSecret,
                grant_type = "client_credentials"
            });
            fb.AccessToken = result.access_token;
            return fb.AccessToken;
        }
    }
}