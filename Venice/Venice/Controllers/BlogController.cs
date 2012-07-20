using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Venice.DTO;
using Venice.Data;
using Venice.Models.Blog;

namespace Venice.Controllers
{
    public class BlogController : Controller
    {

        public ActionResult Index()
        {
            var model = new BlogrollModel();
            model.Posts = BlogQueries.GetAllPosts();
            return View(model);
        }

        public ActionResult View(int id)
        {
            var model = new BlogPostModel();
            model.Post = BlogQueries.GetPostById(id);
            model.Comments = BlogQueries.GetAllCommentsByPostId(id);
            model.NewComment.PostId = id;
            return View(model);
        }

        public ActionResult AddComment(CommentDto NewComment)
        {
            var success = BlogQueries.CreateComment(NewComment);
           if (success)
               TempData["Message"] = "Your comment was successfully posted";
           return RedirectToAction("View", new { id = NewComment.PostId });
        }

        [Authorize]
        public ActionResult NewPost()
        {
            return View("Post");
        }

        [Authorize]
        public ActionResult EditPost(int id)
        {
            var model = BlogQueries.GetPostById(id);
            return View("Post", model);
        }
       
        [Authorize]
        public ActionResult PostBlog(PostDto Post)
        {
            Post.CreatedBy = Post.UpdatedBy = "Authorized user";
            if (Post.PostId > 0)
            {
                var success = BlogQueries.UpdatePost(Post);
                if (success)
                    TempData["Message"] = "Your post was successfully updated";
            }
            else
            {
              var postId =  BlogQueries.CreatePost(Post);
                Post.PostId = postId;
              if (postId > 0)
                    TempData["Message"] = "Your post was successfully posted";
            }

            return RedirectToAction("View", new {id = Post.PostId});
        }

        [Authorize]
        public ActionResult DeletePost(int id)
        {
            BlogQueries.DeletePost(id);
            return RedirectToAction("Index");
        }

    }
}
