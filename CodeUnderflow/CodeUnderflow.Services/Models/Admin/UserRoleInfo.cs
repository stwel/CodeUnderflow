﻿using CodeUnderflow.Common.AutoMapper;
using CodeUnderflow.Data.Models;
using System.Collections.Generic;

namespace CodeUnderflow.Services.Models.Admin
{
    public class UserRoleInfo : IMapFrom<User>
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string Username { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<string> Roles { get; set; } = new List<string>();
    }
}