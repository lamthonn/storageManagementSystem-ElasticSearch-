using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATN.Application.Utils
{
    public class Helper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Helper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public (string? userId, string? userName) GetUserInfo()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return (null, null);

            context.Request.Headers.TryGetValue("X-User-Id", out var userId);
            context.Request.Headers.TryGetValue("X-User-Name", out var userName);

            return (userId.ToString(), userName.ToString());
        }
    }
}
