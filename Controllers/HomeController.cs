using Microsoft.AspNetCore.Mvc;
using SCAP.ViewModels;

namespace SCAP.Controllers
{
    public class HomeController : Controller
    {
        [Route("/error/{code:length(3,3)}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int code)
        {
            if (code == 500)
            {
                return View("Error", new ErrorViewModel
                {
                    Title = "Oops! Ocorreu um erro.",
                    Message = "Tente novamente ou contate a secretaria.",
                    Code = code
                });
            }
            
            if (code == 404)
            {
                return View("Error", new ErrorViewModel
                {
                    Title = "Oops! Conteúdo não encontrado.",
                    Message = "O conteúdo que você está procurando não existe! Em caso de dúvidas, contate a secretaria.",
                    Code = code
                });
            }
            
            if (code == 403)
            {
                return View("Error", new ErrorViewModel
                {
                    Title = "Oops! Acesso Negado.",
                    Message = "Você não tem permissão para fazer isto.",
                    Code = code
                });
            }
            
            return View("Error", new ErrorViewModel
            {
                Title = "Oops! Ocorreu um erro.",
                Message = "Lamentamos pelo ocorrido! Tente novamente ou contate a secretaria.",
                Code = code
            });
        }
    }
}
