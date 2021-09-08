using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace AppIdentity
{
    public class AuthOptions
    {
        public const string ISSUER = "ServerAuth"; // издатель токена
        public const string AUDIENCE = "ClientAuth"; // потребитель токена
        const string KEY = "mykey!86768725!secretkey!hvhj";   // ключ для шифрации
        public const int LIFETIME = 1; // время жизни токена - 1 минута
        public static readonly SymmetricSecurityKey Key;

        static AuthOptions()
        {
            Key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}