namespace Coats.Crafts.Repositories.Interfaces
{
    using Models;
    using System.Collections.Generic;

    public interface ICommentRepository
    {
        string AddComment(string comment, string itemID, string userId, string userName);
        string AddRating(string comment, string itemID, string userId, string displayName);
        string CheckRating(string itemUri, string user, bool remove);
        IList<CDS.Comment> GetAllCommentsByUser(string itemID, string username, int numberToReturn);
        int GetCommentCount(string itemID);
        IList<CDS.Comment> GetComments(string itemID, int numberToReturn);
        IList<CDS.Comment> GetCommentsByUser(string itemID, string username, int numberToReturn);
        string GetRating(string itemID);
    }
}

