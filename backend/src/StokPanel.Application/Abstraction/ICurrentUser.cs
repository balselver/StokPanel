using System;
using System.Collections.Generic;
using System.Text;

namespace StokPanel.Application.Abstraction
{
    public interface ICurrentUser
    {
        string? UserId { get; }
        string? UserName { get; }
        bool IsInRole(string role);
    }
}
