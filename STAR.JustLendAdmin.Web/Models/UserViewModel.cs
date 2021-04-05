namespace STAR.JustLendAdmin.Web.Models
{
    public class UserViewModel
    {
        public User User { get; set; } = new User();
        public ResponseCore Response { get; set; } = new ResponseCore();
    }
}