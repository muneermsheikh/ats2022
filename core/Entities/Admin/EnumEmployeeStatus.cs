using System.Runtime.Serialization;

namespace core.Entities.Admin
{
    public enum EnumEmployeeStatus
    {
        [EnumMember(Value="Employed")]
        Employed,
        [EnumMember(Value="On Leave")]
        OnLeave,
        [EnumMember(Value="Resigned")]
        Resigned,
        [EnumMember(Value="Terminated")]
        Terminated,
        [EnumMember(Value="Absconded")]
        Absconded
    }
}