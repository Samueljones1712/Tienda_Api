using System.Security.Cryptography;
using System.Text;

namespace TiendaApi_Users.Services
{
    public class Encrypt
    {
        public string EncryptPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convertir la contraseña en un arreglo de bytes
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                // Calcular el hash SHA256 de la contraseña
                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                // Convertir el hash en una cadena hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }

                // Devolver la cadena hexadecimal como contraseña encriptada
                return builder.ToString();
            }
        }

        public bool VerifyPassword(string password, string encryptedPassword)
        {
            // Encriptar la contraseña sin encriptar utilizando el mismo método
            string hashedPassword = EncryptPassword(password);

            // Comparar los hashes resultantes
            return string.Equals(hashedPassword, encryptedPassword, StringComparison.OrdinalIgnoreCase);
        }
    }
}

