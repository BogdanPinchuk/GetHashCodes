using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GetCheckHash
{
    class Program
    {
        /// <summary>
        /// Локалізація інтерфейсу
        /// </summary>
        enum Localization
        {
            /// <summary>
            /// Англійська
            /// </summary>
            English,
            /// <summary>
            /// Українська
            /// </summary>
            Ukrainian,
        }

        /// <summary>
        /// Алгоритм розрахунку хеш-кодів
        /// </summary>
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

            // створення словника хеш-кодів всіх файлів в папці
            Dictionary<string, string> hashCodes = new();

            // алгоритм розрахунку
            HashNames algorithm = HashNames.MD5;

            while (true)
            {
                // запит на вибір
                Console.WriteLine("What do you want?\n1. Checking files with md5 hash files?\n2. Create md5 hash file for each file?\n0. Exit.\n");
                ConsoleKey key = Console.ReadKey(true).Key;

                // Перевірка файлів
                if (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1)
                {
                    // директорія
                    string path = Directory.GetCurrentDirectory();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Current directory:\n{path}\n");
                    Console.ResetColor();

                    // список файлів і хеш-файлів в каталогах і підкаталогах
                    List<string> listFileNames = new();
                    List<string> listHashNames = new();
                    listFileNames.AddRange(Directory.GetFiles(path, "*", SearchOption.AllDirectories));
                    // копіювання
                    listHashNames.AddRange(listFileNames);

                    listFileNames.RemoveAll(i => new FileInfo(i).Extension.ToLower() ==
                        $".{Enum.GetName(algorithm).ToString().ToLower()}");
                    listHashNames.RemoveAll(i => new FileInfo(i).Extension.ToLower() !=
                        $".{Enum.GetName(algorithm).ToString().ToLower()}");

                    string[] fileNames = listFileNames.ToArray(),
                        hashNames = listHashNames.ToArray();

                    listFileNames.Clear();
                    //listHashNames.Clear();

                    // сповіщення про відсутність файлів
                    if (fileNames.Length != hashNames.Length)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Not all files have a hash file!");
                        Console.ResetColor();
                    }

                    // розрахунок хеш-кодів
                    hashCodes.Clear();
                    for (int i = 0; i < fileNames.Length; i++)
                        hashCodes.Add(fileNames[i], GetHashCode(fileNames[i], algorithm));

                    // перевірка хеш-файлів
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Start checking hash-files:");
                    Console.ResetColor();

                    foreach (var hashName in listHashNames)
                    {
                        try
                        {
                            string fileName = hashName.Replace($".{Enum.GetName(algorithm).ToString().ToLower()}", "");
                            CheckData(hashName, hashCodes[fileName], algorithm);
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(ex.Message);
                            Console.ResetColor();
                        }
                    }

                    // завершення роботи
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("All hash files were checked!\n");
                    Console.ResetColor();
                    // чекати до виходу
                    Console.ReadKey(true);
                    break;
                }
                // Створення файлів
                else if (key == ConsoleKey.D2 || key == ConsoleKey.NumPad2)
                {
                    // директорія
                    string path = Directory.GetCurrentDirectory();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Current directory:\n{path}\n");
                    Console.ResetColor();

                    // список файлів в каталогах і підкаталогах
                    List<string> listFileNames = new();
                    listFileNames.AddRange(Directory.GetFiles(path, "*", SearchOption.AllDirectories));
                    listFileNames.RemoveAll(i => new FileInfo(i).Extension.ToLower() ==
                        $".{Enum.GetName(algorithm).ToString().ToLower()}"); ;
                    string[] fileNames = listFileNames.ToArray();
                    listFileNames.Clear();

                    // розрахунок хеш-кодів
                    hashCodes.Clear();
                    for (int i = 0; i < fileNames.Length; i++)
                        hashCodes.Add(fileNames[i], GetHashCode(fileNames[i], algorithm));

                    // створення хеш-файлів
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Start creating hash-files:");
                    Console.ResetColor();
                    foreach (var hash in hashCodes)
                        SaveData(hash.Key, hash.Value, algorithm);

                    // завершення роботи
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("All hash files were created succesful!\n");
                    Console.ResetColor();

                    // чекати до виходу
                    Console.ReadKey(true);
                    break;
                }
                // вихід
                else if (key == ConsoleKey.D0 || key == ConsoleKey.NumPad0)
                {
                    break;
                }
                // повторний запит
                else
                {
                    Console.Clear();
                    continue;
                }

            }
            #region MultiThreading
            //// синхронізація доступу
            //object block = new object();

            // багатопотоковість
            //Parallel.For(0, fileNames.Length,
            //    i =>
            //    {
            //        lock (block)
            //        {
            //            hashCodes.Add(fileNames[i], GetHashCode(fileNames[i], algorithm));
            //        }
            //    });
            #endregion
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

        /// <summary>
        /// Збереження даних хеш-кодів
        /// </summary>
        /// <param name="fileName">шлях до файлу</param>
        /// <param name="hashcode">хеш-код</param>
        /// <param name="hashName">метод</param>
        private static void SaveData(string fileName, string hashcode, HashNames hashName)
        {
            // назва нового файлу
            string newName = fileName + ".md5";

            try
            {
                using (StreamWriter sw = new(newName, false, Encoding.UTF8))
                {
                    string data = hashcode + " *" + new FileInfo(fileName).Name;
                    sw.Write(data);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(data);
                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// Перевірка хешів файлів
        /// </summary>
        /// <param name="fileName">шлях до хеш-файлу</param>
        /// <param name="hashcode">хеш-код</param>
        /// <param name="hashName">метод</param>
        private static void CheckData(string fileName, string hashcode, HashNames hashName)
        {
            try
            {
                using (StreamReader sr = new StreamReader(fileName, Encoding.UTF8))
                {
                    string data = sr.ReadToEnd();
                    string header = string.Empty;

                    // перевірка хеш-коду
                    if (data.Contains(hashcode))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        header = "correct file: ";
                        Console.WriteLine(header + data);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        header = "incorrect file: ";
                        Console.WriteLine(header + data);
                        Console.WriteLine("hash code file: " + hashcode);
                    }

                    Console.ResetColor();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

    }
}
