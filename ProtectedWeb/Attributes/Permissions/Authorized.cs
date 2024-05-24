using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using ProtectedWeb.Utils;

namespace ProtectedWeb.Attributes.Permissions
{
    public class Authorized : Attribute, IPageFilter
    {
        public void OnPageHandlerExecuting(PageHandlerExecutingContext filterContext)
        {
            string? token = filterContext.HttpContext.Request.Cookies["access_token"];
            if (token == null)
            {
                filterContext.Result = new UnauthorizedResult();
            }
            if (UserMethods.ValidateToken(token) == null)
            {
                filterContext.Result = new UnauthorizedResult();
            }
        }
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {

        }
        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {

        }
    }
}
