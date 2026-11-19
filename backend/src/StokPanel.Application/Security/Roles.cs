using System;
using System.Collections.Generic;
using System.Text;

namespace StokPanel.Application.Security
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Personel = "Personel";

        public static readonly string[] All = [Admin, Personel];
    }

    public static class Policies
    {
        public const string AdminOnly = "AdminOnly";
    }
}
