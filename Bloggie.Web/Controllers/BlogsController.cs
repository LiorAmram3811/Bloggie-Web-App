using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
using Bloggie.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bloggie.Web.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogPostRepository _blogPostRepository;
        private readonly IBlogPostLikeRepository _blogPostLikeRepository;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IBlogPostCommentRepository _blogPostCommentRepository;

        public BlogsController(IBlogPostRepository blogPostRepository, IBlogPostLikeRepository blogPostLikeRepository,
            SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IBlogPostCommentRepository blogPostCommentRepository)
        {
            _blogPostRepository = blogPostRepository;
            _blogPostLikeRepository = blogPostLikeRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _blogPostCommentRepository = blogPostCommentRepository;
        }
        public async Task<IActionResult> Index(string urlHandle)
        {
            var isLiked = false;
            var post = await _blogPostRepository.GetByUrlHandleAsync(urlHandle);
            var blogDetailsRequest = new BlogDetailsRequest();
            if (post != null)
            {
                var totalLikes = await _blogPostLikeRepository.GetTotalLikes(post.Id);

                if (_signInManager.IsSignedIn(User))
                {
                    var likesForBlog = await _blogPostLikeRepository.GetLikesForBlog(post.Id);
                    var userId = _userManager.GetUserId(User);
                    if (userId != null)
                    {
                        var likeFromUser = likesForBlog.FirstOrDefault(x => x.UserId == Guid.Parse(userId));
                        isLiked = likeFromUser != null;
                    }
                }

                var blogCommentsModel = await _blogPostCommentRepository.GetCommentsByBlogIdAsync(post.Id);
                var blogCommentForView = new List<BlogCommentRequest>();
                foreach (var blogComment in blogCommentsModel)
                {
                    blogCommentForView.Add(new BlogCommentRequest
                    {
                        Description = blogComment.Description,
                        DateAdded = blogComment.DateAdded,
                        Username = (await _userManager.FindByIdAsync(blogComment.UserId.ToString())).UserName
                    });
                }

                blogDetailsRequest = new BlogDetailsRequest
                {
                    Id = post.Id,
                    Content = post.Content,
                    PageTitle = post.PageTitle,
                    Author = post.Author,
                    FeaturedImageUrl = post.FeaturedImageUrl,
                    Heading = post.Heading,
                    PublishedDate = post.PublishedDate,
                    ShortDescription = post.ShortDescription,
                    UrlHandle = post.UrlHandle,
                    IsVisible = post.IsVisible,
                    Tags = post.Tags,
                    TotalLikes = totalLikes,
                    IsLiked = isLiked,
                    Comments = blogCommentForView
                };
            }
            return View(blogDetailsRequest);
        }

        [HttpPost]
        public async Task<IActionResult> Index(BlogDetailsRequest blogDetailsRequest)
        {
            if (_signInManager.IsSignedIn(User))
            {
                var domainModel = new BlogPostComment
                {
                    BlogPostId = blogDetailsRequest.Id,
                    Description = blogDetailsRequest.CommentDescription,
                    UserId = Guid.Parse(_userManager.GetUserId(User)),
                    DateAdded = DateTime.Now
                };

                await _blogPostCommentRepository.AddAsync(domainModel);
                return RedirectToAction("Index", "Blogs", new { urlHandle = blogDetailsRequest.UrlHandle });
            }

            return View();
        }

    }
}
