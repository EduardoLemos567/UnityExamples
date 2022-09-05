using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Game
{
    /// <summary>
    /// Simple, possible insecure string cryptography. So you can encrypt data you dont
    /// wanna your players messing around, like save files.
    /// WARNING: Dont use on any sensitive data. Study the topic, read the specialists 
    /// and do your own class. I'm not responsible for any loss or harm this code can bring you :)
    /// </summary>
    public static class StringCipher
    {
        // This constant is used to determine the keysize of the encryption algorithm in bits.
        // We divide this by 8 within the code below to get the equivalent number of bytes.
        const int BLOCK_SIZE = 256;
        const int KEY_SIZE = BLOCK_SIZE / 8;
        // This constant determines the number of iterations for the password bytes generation function.
        const int DERIVATION_ITERATIONS = 1000;
        public static string Encrypt(string data_string, string password)
        {
            // Array allocations: temp_bytes, data_bytes, key_bytes, cipher_bytes
            var temp_bytes = new byte[KEY_SIZE];
            // Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
            // so that they can be used when decrypting.  
            // string -> byte[]
            var data_bytes = Encoding.UTF8.GetBytes(data_string);
            // Generate salt to be used on key derivation
            GenerateRandom(temp_bytes);
            using (var derivated_password = new Rfc2898DeriveBytes(password, temp_bytes, DERIVATION_ITERATIONS))
            {
                using (var symmetric_key = new RijndaelManaged { BlockSize = BLOCK_SIZE, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 })
                {
                    var key_bytes = derivated_password.GetBytes(KEY_SIZE);
                    // Generate IV to be used on encryption
                    GenerateRandom(temp_bytes);
                    using (var encryptor = symmetric_key.CreateEncryptor(key_bytes, temp_bytes))
                    {
                        using (var mem_stream = new MemoryStream())
                        {
                            using (var crypto_stream = new CryptoStream(mem_stream, encryptor, CryptoStreamMode.Write))
                            {
                                crypto_stream.Write(data_bytes);
                                crypto_stream.FlushFinalBlock();
                                var cipher_bytes = new byte[KEY_SIZE * 2 + (int)mem_stream.Length];
                                // Copy the Salt and the IV on the begining of the array/string
                                Array.Copy(derivated_password.Salt, 0, cipher_bytes, 0, KEY_SIZE);
                                Array.Copy(temp_bytes, 0, cipher_bytes, KEY_SIZE, KEY_SIZE);
                                mem_stream.Position = 0;
                                mem_stream.Read(cipher_bytes, KEY_SIZE * 2, (int)mem_stream.Length);
                                return Convert.ToBase64String(cipher_bytes);
                            }
                        }
                    }
                }
            }
        }
        public static string Decrypt(string ciphered_string, string password)
        {
            // Array allocations: temp_bytes, full_cipher_bytes, key_bytes
            var temp_bytes = new byte[KEY_SIZE];
            // Get the complete stream of bytes that represent:
            // [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
            var full_cipher_bytes = Convert.FromBase64String(ciphered_string);
            // Copy salt[32] from full_cipher_bytes into temp_bytes
            Array.Copy(full_cipher_bytes, 0, temp_bytes, 0, KEY_SIZE);
            using (var derivated_password = new Rfc2898DeriveBytes(password, temp_bytes, DERIVATION_ITERATIONS))
            {
                var key_bytes = derivated_password.GetBytes(KEY_SIZE);
                using (var symmetric_key = new RijndaelManaged { BlockSize = BLOCK_SIZE, Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 })
                {
                    // Copy IV[32] from full_cipher_bytes into temp_bytes
                    Array.Copy(full_cipher_bytes, KEY_SIZE, temp_bytes, 0, KEY_SIZE);
                    using (var decryptor = symmetric_key.CreateDecryptor(key_bytes, temp_bytes))
                    {
                        // Start MemoryStream from full_cipher_bytes, offsetting the first 32*2 bytes: salt+iv
                        using (var mem_stream = new MemoryStream(full_cipher_bytes, KEY_SIZE * 2, full_cipher_bytes.Length - KEY_SIZE * 2))
                        {
                            using (var crypto_stream = new CryptoStream(mem_stream, decryptor, CryptoStreamMode.Read))
                            {
                                var decrypted_byte_count = crypto_stream.Read(full_cipher_bytes, 0, (int)mem_stream.Length);
                                return Encoding.UTF8.GetString(full_cipher_bytes, 0, decrypted_byte_count);
                            }
                        }
                    }
                }
            }
        }
        static void GenerateRandom(byte[] random_bytes)
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                // Fill the array with cryptographically secure random bytes.
                rng.GetBytes(random_bytes);
            }
        }
    }
}