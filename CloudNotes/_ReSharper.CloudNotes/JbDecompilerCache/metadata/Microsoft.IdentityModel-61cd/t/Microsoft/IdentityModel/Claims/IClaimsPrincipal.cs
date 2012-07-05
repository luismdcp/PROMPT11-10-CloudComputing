// Type: Microsoft.IdentityModel.Claims.IClaimsPrincipal
// Assembly: Microsoft.IdentityModel, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// Assembly location: C:\Program Files\Reference Assemblies\Microsoft\Windows Identity Foundation\v3.5\Microsoft.IdentityModel.dll

using System.Security.Principal;

namespace Microsoft.IdentityModel.Claims
{
    public interface IClaimsPrincipal : IPrincipal
    {
        IClaimsPrincipal Copy();
        ClaimsIdentityCollection Identities { get; }
    }
}
