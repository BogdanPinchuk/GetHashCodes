using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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
            /// <summary>
            /// Російська
            /// </summary>
            Russian,
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
        private delegate HashAlgorithm Provider();

        /// <summary>
        /// Лічильник кількості помилок
        /// </summary>
        private static long counter;

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
                Console.WriteLine("What do you want?\n1. Checking files with md5 hash files?\n2. Create md5 hash file for each file?\n9. Delete all hash files!\n0. Exit.\n");
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

                    // всі файли в папці
                    listFileNames.AddRange(Directory.GetFiles(path, "*", SearchOption.AllDirectories));
                    // копіювання
                    listHashNames.AddRange(listFileNames);

                    listFileNames.RemoveAll(i => new FileInfo(i).Extension.ToLower() ==
                        $".{Enum.GetName(algorithm).ToString().ToLower()}");
                    listHashNames.RemoveAll(i => new FileInfo(i).Extension.ToLower() !=
                        $".{Enum.GetName(algorithm).ToString().ToLower()}");

                    // видалення розширень файлів
                    listHashNames = listHashNames
                        .Select(i => i.Replace($".{Enum.GetName(algorithm).ToString().ToLower()}", string.Empty))
                        .ToList();

                    string[] fileNames = listFileNames.ToArray(),
                        hashNWE = listHashNames.ToArray(),
                        hashE, fileE, fileIn;

                    // різниці двох колекцій
                    // відсутність хеш-файлів
                    hashE = fileNames.Except(hashNWE).ToArray();
                    // відсутність файлів
                    fileE = hashNWE.Except(fileNames).ToArray();
                    // наявні файли з хешами
                    fileIn = fileNames.Intersect(hashNWE).ToArray();

                    listFileNames.Clear();
                    listHashNames.Clear();

                    // при відсутності хеш-файлів
                    if (hashE.Length > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Missing {hashE.Length} hash files:\n");
                        Console.ForegroundColor = ConsoleColor.Red;
                        foreach (var file in hashE)
                            Console.WriteLine($"{file}.{Enum.GetName(algorithm).ToString().ToLower()}");
                        Console.WriteLine();
                    }

                    // при відсутності файлів
                    if (fileE.Length > 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Missing {fileE.Length} files:\n");
                        Console.ForegroundColor = ConsoleColor.Red;
                        foreach (var file in fileE)
                            Console.WriteLine($"{file}");
                        Console.WriteLine();
                    }

                    // якщо є відповідні файшли з хешами
                    if (fileIn.Length > 0)
                    {
                        // скидання лічильника
                        counter = 0;

                        // розрахунок хеш-кодів
                        hashCodes.Clear();
                        for (int i = 0; i < fileIn.Length; i++)
                            hashCodes.Add(fileIn[i], GetHashCode(fileIn[i], algorithm));

                        // перевірка хеш-файлів
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Start checking {fileIn.Length} files with hash-files:");
                        Console.ResetColor();

                        for (int i = 0; i < fileIn.Length; i++)
                        {
                            try
                            {
                                string hashName = $"{fileIn[i]}.{Enum.GetName(algorithm).ToString().ToLower()}";
                                CheckData(hashName, fileIn[i], hashCodes[fileIn[i]]);
                            }
                            catch (Exception ex)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine(ex.Message);
                                Console.ResetColor();
                            }
                        }

                        // розрахунок проблемних файлів
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Found {counter}/{fileIn.Length} error files!\n");
                        Console.ResetColor();
                    }
                    
                    // завершення роботи
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("All files were checked!\n");
                    Console.ResetColor();

                    // чекати до виходу
                    Console.ReadKey(true);
                    //break;
                    Console.WriteLine();
                    continue;
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
                    //break;
                    Console.WriteLine();
                    continue;
                }
                // видалення всіх хеш-файлів
                else if (key == ConsoleKey.D9 || key == ConsoleKey.NumPad9)
                {
                    // директорія
                    string path = Directory.GetCurrentDirectory();
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"Current directory:\n{path}\n");
                    Console.ResetColor();

                    // список файлів і хеш-файлів в каталогах і підкаталогах
                    List<string> listHashNames = new();
                    listHashNames.AddRange(Directory.GetFiles(path, "*", SearchOption.AllDirectories));

                    listHashNames.RemoveAll(i => new FileInfo(i).Extension.ToLower() !=
                        $".{Enum.GetName(algorithm).ToString().ToLower()}");

                    string[] fileNames = listHashNames.ToArray(),
                        hashNames = listHashNames.ToArray();

                    listHashNames.Clear();

                    // видалення файлів
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Start deleting hash-files:");
                    Console.ResetColor();
                    try
                    {
                        foreach (var file in fileNames)
                        {
                            File.Delete(file);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"delete: {file}");
                            Console.ResetColor();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(ex.Message);
                        Console.ResetColor();
                    }

                    // завершення роботи
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("All hash files were deleted!\n");
                    Console.ResetColor();

                    // чекати до виходу
                    Console.ReadKey(true);
                    //break;
                    Console.WriteLine();
                    continue;
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

            // провайдер
            Provider provider;

            // розрахунок хеш-коду
            try
            {
                // instance algorithm
                provider = hashName switch
                {
                    HashNames.SHA1 => SHA1.Create,
                    HashNames.MD5 => MD5.Create,
                    HashNames.SHA256 => SHA256.Create,
                    HashNames.SHA384 => SHA384.Create,
                    HashNames.SHA512 => SHA512.Create,
                    _ => MD5.Create,
                };

                //using (FileStream fs = File.OpenRead(path))
                //using (HashAlgorithm algorithm = HashAlgorithm
                //    .Create(Enum.GetName(hashName).ToString()))
                using (FileStream fs = new FileInfo(path).OpenRead())
                using (HashAlgorithm algorithm = provider())
                {
                    algorithm?.Initialize();
                    byte[] checkSum = algorithm?.ComputeHash(fs);
                    result = BitConverter
                        .ToString(checkSum)
                        .Replace("-", string.Empty)
                        .ToLower();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
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
            string newName = fileName + $".{Enum.GetName(hashName).ToString().ToLower()}";

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
        /// <param name="hashfileName">шлях до хеш-файлу</param>
        /// <param name="fileName">шлях до файлу</param>
        /// <param name="hashcode">хеш-код</param>
        private static void CheckData(string hashfileName, string fileName, string hashcode)
        {
            try
            {
                using (StreamReader sr = new(hashfileName, Encoding.UTF8))
                {
                    // зчитані дані із хеш-файла
                    string data = sr.ReadToEnd();
                    // заголовок для виведення інформації
                    string header = string.Empty;
                    // назва файлу розрахованого хеша
                    string name = new FileInfo(fileName).Name;

                    // шлях до файлу
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"file cheking: {fileName}");
                    Console.ResetColor();

                    // регулярні вирази
                    // хеш-код
                    Regex regex = new(@"\S+\s\*");
                    string hash = regex.Match(data).Value.Replace(" *", string.Empty);

                    // назва файлу
                    regex = new(@"\s\*\S*");
                    string findName = regex.Match(data).Value.Replace(" *", string.Empty);

                    // перевірка хеш-коду
                    //if (data.Contains(hashcode) && data.Contains(name))
                    if (hashcode == hash && name == findName)
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
                        Console.WriteLine($"hash code file: {hashcode} *{name}");
                        // враховуємо помилку
                        counter++;
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
