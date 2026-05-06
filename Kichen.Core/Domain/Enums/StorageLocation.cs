using System.ComponentModel;

namespace Kitchen.Core.Domain.Enums
{
    public enum StorageLocation
    {
        [Description("-")]
        Unspecified = 0,

        [Description("lodówka")]
        Fridge = 1,

        [Description("zamrażarka")]
        Freezer = 2,

        [Description("szafki")]
        Pantry = 3
    }
}
