using System;
using System.Collections.Generic;
using System.Text;

namespace StokPanel.Application.Common
{
    public sealed class NotFoundException(string message) : Exception(message);
    
    
}
