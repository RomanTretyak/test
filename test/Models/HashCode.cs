using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace test.Models
{
    public interface IHashCode
    {
        Guid GetHashString(string s);
    }

    public class HashCode : IHashCode
    {
        public Guid GetHashString(string s)
        {
            try
            {
                var bytes = Encoding.Unicode.GetBytes(s);

                var csp =
                    new MD5CryptoServiceProvider();

                var byteHash = csp.ComputeHash(bytes);

                var hash = string.Empty;

                foreach (var b in byteHash)
                    hash += string.Format("{0:x2}", b);

                return new Guid(hash);
            }
            catch (Exception ex)
            {
                Logger.GetLogger().LogError(ex.ToString());
                throw;
            }

        }
    }
}