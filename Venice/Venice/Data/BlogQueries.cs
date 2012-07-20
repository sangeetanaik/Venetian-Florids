using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Venice.ConnectionFactory;
using Venice.DTO;

namespace Venice.Data
{
    public static class BlogQueries
    {
        #region Get
        
        public static List<PostDto> GetAllPosts()
        {
            var blogRoll = new List<PostDto>();
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT * FROM POSTS ORDER BY UpdatedOn DESC", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        blogRoll.Add(ReadPost(dr));
                    }
                }
                conn.Close();
            }
            return blogRoll;
        }

        public static PostDto GetPostById(int postId)
        {
            var post = new PostDto();
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT * FROM POSTS WHERE PostId = " + postId, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        post = ReadPost(dr);
                    }
                }
                conn.Close();
            }
            return post;
        }

        public static List<CommentDto> GetAllCommentsByPostId(int postId)
        {
            var comments = new List<CommentDto>();
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT * FROM COMMENTS WHERE PostId =" + postId + " ORDER BY CommentId desc", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        comments.Add(ReadComment(dr));
                    }
                }
                conn.Close();
            }
            return comments;
        }

        public static int GetCommentCountByPostId(int postId)
        {
            int commentsCount = 0;
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT COUNT(*) AS CommentCount FROM COMMENTS WHERE PostId =" + postId, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        commentsCount = dr["CommentCount"] != DBNull.Value ? Convert.ToInt32(dr["CommentCount"]) : 0;
                    }
                }
                conn.Close();
            }
            return commentsCount;
        }

        #endregion

        #region Insert/Update/Delete

        public static int CreatePost(PostDto post)
        {
            var id = 0;
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var createCommand = "INSERT INTO Posts (Title,Body,CreatedOn,CreatedBy,UpdatedOn,UpdatedBy) VALUES ('"
                                    + post.Title.Replace("'", "''") + "', '"
                                    + post.Body.Replace("'", "''")+ "', "
                                    + DateTime.Now.ToShortDateString() + ", '"
                                    + post.CreatedBy + "', "
                                    + DateTime.Now.ToShortDateString() + ", '"
                                    + post.UpdatedBy + "'); SELECT SCOPE_IDENTITY()";

                using (var cmd = new SqlCommand(createCommand, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    id = Convert.ToInt32(cmd.ExecuteScalar());
                }
                conn.Close();
            }
            return id;
        }

        public static bool UpdatePost(PostDto post)
        {
            var rowsAffected = 0;
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var updateCommand = "UPDATE Posts SET  Title = '" + post.Title.Replace("'", "''") +
                                    "',Body = '" + post.Body.Replace("'", "''") +
                                    "',UpdatedOn = '" + DateTime.Now +
                                    "',UpdatedBy = '" + post.UpdatedBy +
                                    "' WHERE PostId = " + post.PostId;

                using (var cmd = new SqlCommand(updateCommand, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return rowsAffected > 0;
        }

        public static void DeletePost(int postId)
        {
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var deleteCommand = "DELETE from Comments WHERE PostId = " + postId + ";"
                    + " DELETE from Posts WHERE PostId = " + postId + ";";
                using (var cmd = new SqlCommand(deleteCommand, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }


        public static bool CreateComment(CommentDto comment)
        {
            var rowsAffected = 0;
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var createCommand = "INSERT INTO Comments (PostId,Message,CreatedOn,CreatedBy) VALUES ("
                                    + comment.PostId + ", '"
                                    + comment.Message + "', '"
                                    + DateTime.Now + "', '"
                                    + comment.CreatedBy + "');" 
                                    + " UPDATE Posts SET CommentsCount = CommentsCount + 1 WHERE PostId = " + comment.PostId;

                using (var cmd = new SqlCommand(createCommand, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return rowsAffected > 0;
        }

        public static void DeleteComments(int postId)
        {
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var deleteCommand = "DELETE from Comments WHERE PostId = " + postId +
                        "; UPDATE Posts SET CommentsCount = 0 WHERE PostId = " + postId;
                using (var cmd = new SqlCommand(deleteCommand, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }

        public static void DeleteComment(int postId, int commentId)
        {
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var deleteCommand = "DELETE from Comments WHERE CommentId = " + commentId +
                        "; UPDATE Posts SET CommentsCount = CommentsCount - 1 WHERE PostId = " + postId;
                using (var cmd = new SqlCommand(deleteCommand, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
        #endregion

        #region Read

        private static PostDto ReadPost(SqlDataReader dr)
        {
            var p = new PostDto
            {
                PostId = Convert.ToInt32(dr["PostId"]),
                Title = dr["Title"] != DBNull.Value ? dr["Title"].ToString() : string.Empty,
                Body = dr["Body"] != DBNull.Value ? dr["Body"].ToString() : string.Empty,
                CreatedBy = dr["CreatedBy"] != DBNull.Value ? dr["CreatedBy"].ToString() : string.Empty,
                UpdatedBy = dr["UpdatedBy"] != DBNull.Value ? dr["UpdatedBy"].ToString() : string.Empty,
                CreatedOn = dr["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(dr["CreatedOn"].ToString()) : DateTime.MinValue,
                UpdatedOn = dr["UpdatedOn"] != DBNull.Value ? Convert.ToDateTime(dr["UpdatedOn"].ToString()) : DateTime.MinValue,
                CommentCount = dr["CommentsCount"] != DBNull.Value ? Convert.ToInt32(dr["CommentsCount"].ToString()) : 0
            };
            return p;
        }

        private static CommentDto ReadComment(SqlDataReader dr)
        {
            var c = new CommentDto
                        {
                            CommentId   = Convert.ToInt32(dr["CommentId"]),
                            Message     = dr["Message"] != DBNull.Value ? dr["Message"].ToString() : string.Empty,
                            CreatedBy   = dr["CreatedBy"] != DBNull.Value ? dr["CreatedBy"].ToString() : string.Empty,
                            CreatedOn   = dr["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(dr["CreatedOn"].ToString()) : DateTime.MinValue
                        };
            return c;
        }

        #endregion
    }
}