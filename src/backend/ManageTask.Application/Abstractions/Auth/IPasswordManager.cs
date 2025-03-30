﻿using ResultSharp.Core;

namespace ManageTask.Application.Abstractions.Auth
{
    public interface IPasswordManager
    {
        Result<string> HashPassword(string password);
        Result<bool> VerifyPassword(string password, string hashedPassword);
    }
}
