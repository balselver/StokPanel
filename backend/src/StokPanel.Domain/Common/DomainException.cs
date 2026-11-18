using System;
using System.Collections.Generic;
using System.Text;

namespace StokPanel.Domain.Common
{
    //iş kuralı ihlalinde hata fırlat
    public sealed class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}
