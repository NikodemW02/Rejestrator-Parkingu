using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Klasa reprezentująca użytkownika
class User
{
    public string Username { get; set; }
    public string Password { get; set; }
}

// Klasa zarządzająca użytkownikami
class UserManager
{
    private static List<User> users = new List<User>();

    public UserManager()
    {
        LoadUsers();
    }

    public static bool Login(string username, string password)
    {
        var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);

        if (user != null)
        {
            Console.WriteLine("Zalogowano pomyślnie.");
            Program.Menu();
            return true; // Dodaj zwracanie true, aby wiedzieć, że użytkownik jest zalogowany.
        }
        else
        {
            Console.WriteLine("Użytkownik o podanej nazwie i haśle nie istnieje w bazie.");
            return false; // Zwracaj false, jeśli nie udało się zalogować.
        }
    }

    public static bool CreateUser(string username, string password)
    {
        if (users.Any(u => u.Username == username))
        {
            Console.WriteLine("Użytkownik o podanej nazwie już istnieje.");
            return false;
        }
        users.Add(new User { Username = username, Password = password });
        SaveUsers();
        Console.WriteLine("Konto zostało utworzone.");
        return true;
    }

    public static bool DeleteUser(string username)
    {
        var user = users.FirstOrDefault(u => u.Username == username);
        if (user != null)
        {
            users.Remove(user);
            SaveUsers();
            Console.WriteLine("Konto zostało usunięte.");
            return true;
        }
        else
        {
            Console.WriteLine("Użytkownik o podanej nazwie nie istnieje.");
            return false;
        }
    }

    public static void LoadUsers()
    {
        if (File.Exists("Users.txt"))
        {
            using (var streamReader = new StreamReader("Users.txt"))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    var data = line.Split(',');
                    if (data.Length == 2)
                    {
                        users.Add(new User
                        {
                            Username = data[0],
                            Password = data[1]
                        });
                    }
                }
            }
        }
    }

    private static void SaveUsers()
    {
        var lines = users.Select(user => $"{user.Username},{user.Password}");
        File.WriteAllLines("Users.txt", lines);
    }
}
