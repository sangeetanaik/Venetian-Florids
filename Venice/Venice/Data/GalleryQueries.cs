using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Venice.ConnectionFactory;
using Venice.DTO;
using Venice.Infrastructure;

namespace Venice.Data
{
    public static class GalleryQueries
    {
        #region Insert/Update/Delete

        public static int UploadImage(ImagesDto image)
        {
            var id = 0;
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var createCommand = "INSERT INTO Images (Title,Description,Url,UploadedOn,UploadedBy, GalleryId) VALUES ('"
                    + (image.Title != null ? image.Title.ToDbCleanString() : image.Title) + "', '"
                                    + (image.Description != null ? image.Description.ToDbCleanString() : image.Description) + "', '"
                                    + "', '"
                                    + DateTime.Now.ToString("M/d/yyyy hh:mm:ss tt") + "', '"
                                    + image.UploadedBy+ "', "
                                    + image.GalleryId + "); SELECT SCOPE_IDENTITY()";

                using (var cmd = new SqlCommand(createCommand, conn))
                { 
                    cmd.CommandType = CommandType.Text;
                    id = Convert.ToInt32(cmd.ExecuteScalar());
                }
                conn.Close();
            }
            return id;
        }

        public static int CreateGallery(GalleryDto gallery)
        {
            var id = 0;
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var createCommand = "INSERT INTO Galleries (Title,Description,CreatedOn,CreatedBy) VALUES ('"
                                    + gallery.Title.ToDbCleanString()  + "', '"
                                    + gallery.Description.ToDbCleanString() + "', '"
                                    + DateTime.Now.ToString("M/d/yyyy hh:mm:ss tt") + "', '"
                                    + gallery.CreatedBy + "'); SELECT SCOPE_IDENTITY()";

                using (var cmd = new SqlCommand(createCommand, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    id = Convert.ToInt32(cmd.ExecuteScalar());
                }
                conn.Close();
            }
            return id;
        }

        public static int AddCoverImage(int galleryId, int imageId)
        {
            var rowsAffected = 0;
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var addImageCommand = "UPDATE Galleries SET CoverImageId = " + imageId + " WHERE Id = " + galleryId;

                using (var cmd = new SqlCommand(addImageCommand, conn))
                {
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return rowsAffected;
        }

        public static void AddImagestoGallery(int galleryId, List<ImagesDto> imageIds)
        {
            foreach (var img in imageIds)
            {
                UploadImage(img);
            }
        }

        public static void RemoveImagesfromGallery(int galleryId, List<int> imageIds)
        {
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var deleteCommand = "DELETE FROM Images WHERE GalleryId = " + galleryId + " AND Id in (" + imageIds.ToCommaDelimitedList() + " )";

                using (var cmd = new SqlCommand(deleteCommand, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        public static int UpdateImageTitleDesc(int imgId, string imgTitle, string imgDesc)
        {
            var rowsAffected = 0;
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                var updateImageCommand = "UPDATE Images SET Title = '" + imgTitle.ToDbCleanString() + "', Description = '" + imgDesc.ToDbCleanString() + "' WHERE Id = " + imgId;

                using (var cmd = new SqlCommand(updateImageCommand, conn))
                {
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return rowsAffected;
        }

        #endregion

        #region Get

        public static List<GalleryDto> GetAllGalleries()
        {
            var galleries = new List<GalleryDto>();
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT g.*, i.Id as ImageId, i.Title as ImageTitle, i.Description as ImageDescription, i.Url as ImageUrl" +
                                                " FROM Galleries g LEFT JOIN Images i on i.Id = g.CoverImageId", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        galleries.Add(ReadGallery(dr));
                    }
                }
                conn.Close();
            }
            return galleries;
        }

        public static GalleryDto GetGalleryById(int galleryId)
        {
            var gallery = new GalleryDto();
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT g.*, i.Id as ImageId, i.Title as ImageTitle, i.Description as ImageDescription, i.Url as ImageUrl" +
                                                " FROM Galleries g LEFT JOIN Images i on i.GalleryId = g.Id WHERE g.Id = " + galleryId, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        gallery = ReadGallery(dr);
                    }
                }
                conn.Close();
            }
            return gallery;
        }

        public static List<ImagesDto> GetAllImagesByGalleryId(int galleryId)
        {
            var galleryImages = new List<ImagesDto>();
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT * FROM Images WHERE GalleryId = " + galleryId, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        galleryImages.Add(ReadImage(dr));
                    }
                }
                conn.Close();
            }
            return galleryImages;
        }

        public static ImagesDto GetImageById(int imageId)
        {
            var image = new ImagesDto();
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT * FROM Images WHERE Id = " + imageId, conn))
                {
                    cmd.CommandType = CommandType.Text;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        image = ReadImage(dr);
                    }
                }
                conn.Close();
            }
            return image;
        }

        public static List<ImagesDto> GetRandomImages(int rowCount)
        {
            var galleryImages = new List<ImagesDto>();
            using (var conn = Connection.GetConnection())
            {
                conn.Open();
                using (var cmd = new SqlCommand("SELECT TOP "+ rowCount +" * FROM Images ORDER BY NEWID()", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        galleryImages.Add(ReadImage(dr));
                    }
                }
                conn.Close();
            }
            return galleryImages;
        }
 

        #endregion

        #region Read
        private static GalleryDto ReadGallery(SqlDataReader dr)
        {
            var g = new GalleryDto
            {
                GalleryId = Convert.ToInt32(dr["Id"]),
                Title = dr["Title"] != DBNull.Value ? dr["Title"].ToString() : string.Empty,
                Description = dr["Description"] != DBNull.Value ? dr["Description"].ToString() : string.Empty,
                CreatedBy = dr["CreatedBy"] != DBNull.Value ? dr["CreatedBy"].ToString() : string.Empty,
                CreatedOn = dr["CreatedOn"] != DBNull.Value ? Convert.ToDateTime(dr["CreatedOn"].ToString()) : DateTime.MinValue,
                CoverImage = new ImagesDto()
                                 {
                                     GalleryId = Convert.ToInt32(dr["Id"]),
                                     ImageId = dr["ImageId"] != DBNull.Value ? Convert.ToInt32(dr["ImageId"]) : 0,
                                     Title = dr["ImageTitle"] != DBNull.Value ? dr["ImageTitle"].ToString() : string.Empty,
                                     Description = dr["ImageDescription"] != DBNull.Value ? dr["ImageDescription"].ToString() : string.Empty,
                                     //Url = dr["ImageUrl"] != DBNull.Value ? dr["ImageUrl"].ToString() : string.Empty
                                 }
            };
            return g;
        }

        private static ImagesDto ReadImage(SqlDataReader dr)
        {
            var i = new ImagesDto
            {
                GalleryId = Convert.ToInt32(dr["GalleryId"]),
                ImageId = Convert.ToInt32(dr["Id"]),
                Title = dr["Title"] != DBNull.Value ? dr["Title"].ToString() : string.Empty,
                Description = dr["Description"] != DBNull.Value ? dr["Description"].ToString() : string.Empty,
                //Url = dr["Url"] != DBNull.Value ? dr["Url"].ToString() : string.Empty,
                UploadedBy = dr["UploadedBy"] != DBNull.Value ? dr["UploadedBy"].ToString() : string.Empty,
                UploadedOn = dr["UploadedOn"] != DBNull.Value ? Convert.ToDateTime(dr["UploadedOn"].ToString()) : DateTime.MinValue
            };
            return i;
        }

        #endregion
    }
}