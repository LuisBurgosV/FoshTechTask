using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sat.Recruitment.Api.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Sat.Recruitment.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public partial class UsersController : ControllerBase
    {
        private readonly List<User> _users = new List<User>();

        [HttpPost]
        [Route("/create-user")]
        public async Task<Result> CreateUser(string name, string email, string address, string phone, string userType, string money)
        {
            var errors = ValidateErrors(name, email, address, phone);

            if (errors != String.Empty)
                return NoneSuccessfulResult(errors);

            try
            {
                email = UsersFactory.NormalizeEmail(email);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return NoneSuccessfulResult("Failed to normalize email");
            }
            
            var newUser = new User
            {
                Name = name,
                Email = email,
                Address = address,
                Phone = phone,
                UserType = userType,
                Money = decimal.Parse(money)
            };

            UsersFactory.NewUser(ref newUser);

            try
            {
                ReadUsers();              
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return NoneSuccessfulResult("Failure to read file");
            }

            if (UsersFactory.CheckDuplicatedUsers(_users, newUser))
                return NoneSuccessfulResult("The user is duplicated");
            else
                return SuccessfulResult("User Created");
        }

        private void ReadUsers()
        {
            var reader = UsersFactory.ReadUsersFromFile();

            while (reader.Peek() >= 0)
            {
                var line = reader.ReadLineAsync().Result;
                var user = new User
                {
                    Name = line.Split(',')[0].ToString(),
                    Email = line.Split(',')[1].ToString(),
                    Phone = line.Split(',')[2].ToString(),
                    Address = line.Split(',')[3].ToString(),
                    UserType = line.Split(',')[4].ToString(),
                    Money = decimal.Parse(line.Split(',')[5].ToString()),
                };
                _users.Add(user);
            }
            reader.Close();
        }

        //Validate errors
        private string ValidateErrors(string name, string email, string address, string phone)
        {
            string errors = String.Empty;

            if (name == null)
                //Validate if Name is null
                errors = "The name is required";
            if (email == null)
                //Validate if Email is null
                errors += " The email is required";
            if (address == null)
                //Validate if Address is null
                errors += " The address is required";
            if (phone == null)
                //Validate if Phone is null
                errors += " The phone is required";

            return errors;
        }

        private Result SuccessfulResult(string message)
        {
            return new Result()
            {
                IsSuccess = true,
                Errors = message
            };
        }

        private Result NoneSuccessfulResult(string message)
        {
            return new Result()
            {
                IsSuccess = false,
                Errors = message
            };
        }
    }
}
