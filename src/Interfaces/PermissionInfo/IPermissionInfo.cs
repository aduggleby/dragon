using System;

namespace Dragon.Interfaces
{
    public interface IPermissionInfo
    {
        String Subject { get; set; }
        String Spec { get; set; }
        bool Inherit { get; set; }
        bool Inherited { get; set; }
    }
}
