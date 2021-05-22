using System.Collections.Generic;

namespace SCAP.ViewModels
{
    public class UserViewModelCollection
    {
        public IEnumerable<UserViewModel> Professores { get; set; }
        public IEnumerable<UserViewModel> Secretarios { get; set; }
    }
}
