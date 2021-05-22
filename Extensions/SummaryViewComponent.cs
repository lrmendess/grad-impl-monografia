using Microsoft.AspNetCore.Mvc;
using SCAP.Notifications;
using System.Linq;
using System.Threading.Tasks;

namespace SCAP.Extensions
{
    public class SummaryViewComponent : ViewComponent
    {
        private readonly INotificator _notificator;

        public SummaryViewComponent(INotificator notificator)
        {
            _notificator = notificator;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var errors = await Task.FromResult(_notificator.GetErrors().ToList());

            // Adiciona erros na ModelState
            errors.ForEach(c => ViewData.ModelState.AddModelError(c.Key, c.Message));

            return View();
        }
    }
}
