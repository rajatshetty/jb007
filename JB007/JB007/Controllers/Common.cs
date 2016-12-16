using System;
using System.Text;

namespace JB007.Controllers
{
    public class Common
    {
        public static string Encrypt(string password)
        {
            string encryptedPassword;
            try
            {
                byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(password);
                encryptedPassword = Convert.ToBase64String(b);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return encryptedPassword;
        }

        public static string Decrypt(string password)
        {
            string decryptedPassword;
            try
            {
                byte[] b = Convert.FromBase64String(password);
                decryptedPassword = System.Text.ASCIIEncoding.ASCII.GetString(b);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return decryptedPassword;
        }
    }
}