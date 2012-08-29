// Type: System.Linq.IQueryable`1
// Assembly: System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.Core.dll

using System.Collections;
using System.Collections.Generic;

namespace System.Linq
{
    [__DynamicallyInvokable]
    public interface IQueryable<out T> : IEnumerable<T>, IQueryable, IEnumerable
    {
    }
}
