namespace Coats.Crafts.Repositories.Interfaces
{
    using System;
    using System.Collections.Generic;

    public interface ICommentRepository
    {
        string AddComment(string comment, string itemID, string userId, string userName);
        string AddRating(string comment, string itemID, string userId, string displayName);
        string CheckRating(string itemUri, string user, bool remove);
        IList<Comment> GetAllCommentsByUser(string itemID, string username, int numberToReturn);
        int GetCommentCount(string itemID);
        IList<Comment> GetComments(string itemID, int numberToReturn);
        IList<Comment> GetCommentsByUser(string itemID, string username, int numberToReturn);
        string GetRating(string itemID);
    }
}

