using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GetCheckHash
{
    class Program
    {
        enum HashNames
        {
            /// <summary>
            /// SHA1CryptoServiceProvider
            /// </summary>
            SHA1,
            /// <summary>
            /// MD5CryptoServiceProvider
            /// </summary>
            MD5,
            /// <summary>
            /// SHA256Managed
            /// </summary>
            SHA256,
            /// <summary>
            /// SHA384Managed
            /// </summary>
            SHA384,
            /// <summary>
            /// SHA512Managed
            /// </summary>
            SHA512,
        }
        //     SHA –System.Security.Cryptography.SHA1CryptoServiceProvider
        //     SHA1 –System.Security.Cryptography.SHA1CryptoServiceProvider
        //     System.Security.Cryptography.SHA1 –System.Security.Cryptography.SHA1CryptoServiceProvider
        //     System.Security.Cryptography.HashAlgorithm –System.Security.Cryptography.SHA1CryptoServiceProvider
        //     MD5 –System.Security.Cryptography.MD5CryptoServiceProvider
        //     System.Security.Cryptography.MD5 –System.Security.Cryptography.MD5CryptoServiceProvider
        //     SHA256 –System.Security.Cryptography.SHA256Managed
        //     SHA-256 –System.Security.Cryptography.SHA256Managed
        //     System.Security.Cryptography.SHA256 –System.Security.Cryptography.SHA256Managed
        //     SHA384 –System.Security.Cryptography.SHA384Managed
        //     SHA-384 –System.Security.Cryptography.SHA384Managed
        //     System.Security.Cryptography.SHA384 –System.Security.Cryptography.SHA384Managed
        //     SHA512 –System.Security.Cryptography.SHA512Managed
        //     SHA-512 –System.Security.Cryptography.SHA512Managed
        //     System.Security.Cryptography.SHA512 –System.Security.Cryptography.SHA512Managed

        static void Main()
        {
            // відображеня кирилиці
            Console.OutputEncoding = Encoding.Unicode;

            // директорія
            string path = Directory.GetCurrentDirectory();

            // каталоги
            //string dirs = Directory.

            // список файлів в каталогах і підкаталогах
            string[] fileNames = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

            // створення словника хеш-кодів всіх файлів в папці
            Dictionary<string, string> hashCodes = new Dictionary<string, string>();

            // обчислення хеш-кодів
            for (int i = 0; i < fileNames.Length; i++)
                hashCodes.Add(fileNames[i], GetHashCode(fileNames[i], HashNames.MD5));


            foreach (var item in hashCodes)
            {
                Console.WriteLine(item.ToString());
            }

            Console.WriteLine("\nМоя папка:");
            Console.WriteLine(path);
            // результати
            //string result = default;



            //Console.WriteLine(result);

            Console.ReadKey(true);
        }

        /// <summary>
        /// Отрмання хеш-коду файлу
        /// </summary>
        /// <param name="path">шлях до файлу</param>
        /// <param name="hashName">метод</param>
        /// <returns>хеш-коди</returns>
        private static string GetHashCode(string path, HashNames hashName)
        {
            // результат хеш-коду
            string result = string.Empty;

            // розрахунок хеш-коду
            try
            {
                using (FileStream fs = File.OpenRead(path))
                using (HashAlgorithm algorithm = HashAlgorithm
                    .Create(Enum.GetName(hashName).ToString()))
                {
                    byte[] checkSum = algorithm.ComputeHash(fs);
                    result = BitConverter
                        .ToString(checkSum)
                        .Replace("-", string.Empty)
                        .ToLower();
                }
            }
            catch (Exception ex)
            {
                var color = Console.BackgroundColor;
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.BackgroundColor = color;
            }

            return result;
        }


    }
}
