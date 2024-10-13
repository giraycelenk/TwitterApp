using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TwitterApp.Data.Abstract;
using TwitterApp.Models;

namespace TwitterApp.ViewComponents.Index
{
    public class IndexTrends:ViewComponent
    {
        
        public IndexTrends()
        {
            
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}