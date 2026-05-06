using System.ComponentModel;

namespace Kitchen.Core.Domain.Enums
{
    
    public enum UnitType
    {
        [Description("-")]
        Unspecified = 0,

        [Description("szt")]
        Pieces = 1,

        [Description("kg")]
        Kilograms = 2,

        [Description("l")]
        Liters = 3
    }
}
