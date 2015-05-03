﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using TestIdentity2.Models;

namespace TestIdentity2.Filters
{
    public class SecureResourceAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            var authorizeHeader = actionContext.Request.Headers.Authorization;
            if (authorizeHeader != null
                && authorizeHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase)
                && String.IsNullOrEmpty(authorizeHeader.Parameter) == false)
            {
                var encoding = Encoding.GetEncoding("ISO-8859-1");
                var credintials = encoding.GetString(
                                   Convert.FromBase64String(authorizeHeader.Parameter));
                string username = credintials.Split(':')[0];
                string password = credintials.Split(':')[1];
                string roleOfUser = string.Empty;
                t5pDBContext db = new t5pDBContext();
                user theUser = db.users.Where(
                    e => (e.usercode == username)).FirstOrDefault();
                //ActivityBAL bal = new ActivityBAL();
                if (theUser != null && theUser.userpassword.CompareTo(password) == 0)
                {
                    var principal = new GenericPrincipal((new GenericIdentity(username)),
                                                                (new[] { roleOfUser }));
                    Thread.CurrentPrincipal = principal;
                    return;
                }
            }
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode
                                                                                   .Unauthorized);

            actionContext.Response.Content = new StringContent("Username and password are missings or invalid");
        }
    } 
}