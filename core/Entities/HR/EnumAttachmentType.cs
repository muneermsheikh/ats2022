using System.Runtime.Serialization;

namespace core.Entities.HR
{
    public enum EnumAttachmentType
    {
        [EnumMember(Value="CV")] CV,
        [EnumMember(Value="Qualification Certificate")] Qualification,
        [EnumMember(Value="Experience Certificate")] Experience,
        [EnumMember(Value="Passport Copy")] Passport
    }
}