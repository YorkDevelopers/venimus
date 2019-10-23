using System;
using System.Security.Cryptography;

namespace CreateTestKeys
{
    class Program
    {
        static void Main(string[] args)
        {
            using var rsa = RSA.Create(2048);

            var parameters = rsa.ExportParameters(true);

            Console.WriteLine("Keysize is " + rsa.KeySize);

            Console.WriteLine($"Modulus={ConvertToBase64String(parameters.Modulus)}");
            Console.WriteLine($"Exponent={ConvertToBase64String(parameters.Exponent)}");
            Console.WriteLine($"D={ConvertToBase64String(parameters.D)}");
            Console.WriteLine($"P={ConvertToBase64String(parameters.P)}");

            System.IO.File.WriteAllText(@"private.xml", ExtractKeys(rsa, true));

        }

        private static string ConvertToBase64String(byte[] value) =>
            Convert.ToBase64String(value).Replace("/", "_").Replace("+", "-").Replace("=", "");


        private static string ExtractKeys(RSA rsa, bool includePrivateParameters)
        {
            var parameters = rsa.ExportParameters(includePrivateParameters);

            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                  parameters.Modulus != null ? ConvertToBase64String(parameters.Modulus) : null,
                  parameters.Exponent != null ? ConvertToBase64String(parameters.Exponent) : null,
                  parameters.P != null ? ConvertToBase64String(parameters.P) : null,
                  parameters.Q != null ? ConvertToBase64String(parameters.Q) : null,
                  parameters.DP != null ? ConvertToBase64String(parameters.DP) : null,
                  parameters.DQ != null ? ConvertToBase64String(parameters.DQ) : null,
                  parameters.InverseQ != null ? ConvertToBase64String(parameters.InverseQ) : null,
                  parameters.D != null ? ConvertToBase64String(parameters.D) : null);
        }
    }

}
