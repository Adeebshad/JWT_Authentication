namespace SimpleJWT.Models
{
    public class UserConstants
    {
        public static List<UserModel> Users = new()
        {
                new UserModel(){ Username="admin",Password="admin",Role="Admin"},
                new UserModel(){ Username="guru",Password="guru",Role="User"}
        };
    }
}
