using Bloggie.Web.Models.ViewModels;
using Bloggie.Web.Repositories;
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

        public BlogsController(IBlogPostRepository blogPostRepository, IBlogPostLikeRepository blogPostLikeRepository,
            SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _blogPostRepository = blogPostRepository;
            _blogPostLikeRepository = blogPostLikeRepository;
            _signInManager = signInManager;
            _userManager = userManager;
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
                    if(userId != null)
                    {
                        var likeFromUser = likesForBlog.FirstOrDefault(x => x.UserId == Guid.Parse(userId));
                        isLiked = likeFromUser != null;
                    }
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
                    IsLiked = isLiked
                };
            }
            return View(blogDetailsRequest);
        }
    }
}
