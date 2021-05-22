using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SCAP.Models;
using SCAP.Notifications;
using SCAP.Services.Interfaces;

namespace SCAP.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IUserService<Pessoa> UserService;
        protected readonly IMapper Mapper;
        protected readonly INotificator Notificator;

        protected BaseController(IUserService<Pessoa> userService, IMapper mapper, INotificator notificator)
        {
            UserService = userService;
            Mapper = mapper;
            Notificator = notificator;
        }

        protected bool ErrorsOccurred()
        {
            return Notificator.HasErrors();
        }

        protected JsonResult JsonOk(string url = "")
        {
            return Json(new { success = true, status = 200, url });
        }

        protected JsonResult JsonNotFound(string url = "")
        {
            return Json(new { success = false, status = 404, url });
        }

        protected JsonResult JsonForbid(string url = "")
        {
            return Json(new { success = false, status = 403, url });
        }

        protected bool IsOwnerOfId(string id)
        {
            return !String.IsNullOrEmpty(id) && (UserService.GetId(User) == id);
        }

        protected bool IsSecretarioOrOwnerOfId(string id)
        {
            return !String.IsNullOrEmpty(id) && (UserService.GetId(User) == id || User.IsInRole("Secretario"));
        }
    }
}
