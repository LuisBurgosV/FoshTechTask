using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Policy;

namespace Sat.Recruitment.Api.Entities
{
    public class UsersFactory
    {
        private const string Normal = "Normal";
        private const string SuperUser = "SuperUser";
        private const string Premium = "Premium";

        public static bool CheckDuplicatedUsers(List<User> _users, User newUser)
        {
            bool isDuplicated = false;

            foreach (var user in _users)
            {
                if (user.Email == newUser.Email || user.Phone == newUser.Phone || user.Name == newUser.Name || user.Address == newUser.Address)
                    isDuplicated = true;
                else isDuplicated = false;
            }
            return isDuplicated;
        }

        public static void NewUser(ref User newUser)
        {
            switch (newUser.UserType)
            {
                case Normal:

                    if (newUser.Money > 100)
                    {
                        CalculateMoneyGif(ref newUser, 0.12);
                    }
                    else if (newUser.Money < 100 && newUser.Money > 10)
                    {
                        CalculateMoneyGif(ref newUser, 0.8);
                    }
                    break;
                case SuperUser:

                    if (newUser.Money > 100)
                    {
                        CalculateMoneyGif(ref newUser, 0.20);
                    }
                    break;
                case Premium:

                    if (newUser.Money > 100)
                    {
                        var gif = newUser.Money * 2;
                        newUser.Money = newUser.Money + gif;
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private static void CalculateMoneyGif(ref User newUser, double decimalPercentage)
        {
            var percentage = Convert.ToDecimal(decimalPercentage);
            var gif = newUser.Money * percentage;
            newUser.Money = newUser.Money + gif;
        }

        public static string NormalizeEmail(string email)
        {
            var aux = email.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
            var atIndex = aux[0].IndexOf("+", StringComparison.Ordinal);
            aux[0] = atIndex < 0 ? aux[0].Replace(".", "") : aux[0].Replace(".", "").Remove(atIndex);

            return string.Join("@", new string[] { aux[0], aux[1] });
        }

        public static StreamReader ReadUsersFromFile()
        {
            var path = Directory.GetCurrentDirectory() + "/Files/Users.txt";
            FileStream fileStream = new FileStream(path, FileMode.Open);
            StreamReader reader = new StreamReader(fileStream);

            return reader;
        }
    }
}
