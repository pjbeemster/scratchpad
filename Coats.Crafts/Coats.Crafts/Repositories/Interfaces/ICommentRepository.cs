using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coats.Crafts.Data;

namespace Coats.Crafts.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        string AddComment(string comment, string itemID, string userId, string userName);
        IList<CDS.Comment> GetComments(string itemID, int numberToReturn);
        IList<CDS.Comment> GetCommentsByUser(string itemID, string username, int numberToReturn);
        IList<CDS.Comment> GetAllCommentsByUser(string itemID, string username, int numberToReturn);
        string GetRating(string itemID);
        string AddRating(string comment, string itemID, string userId, string displayName);
        int GetCommentCount(string itemID);
        string CheckRating(string itemUri, string user, bool remove);
    }
}
