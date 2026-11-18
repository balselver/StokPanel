using System;
using System.Collections.Generic;
using System.Text;

namespace StokPanel.Domain.Common
{
    //her nesneye benzersiz bir ID
    public abstract class Entity
    {
        public Guid Id { get; protected set; } = Guid.CreateVersion7();
    }
}
