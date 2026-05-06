using System.ComponentModel;

namespace Kitchen.Core.Domain.Enums
{
    public enum Category
    {
        [Description("-")]
        Unspecified = 0,

        [Description("mięso")]
        Meat = 1,

        [Description("warzywa")]
        Vegetables = 2,

        [Description("nabiał")]
        Dairy = 3,

        [Description("sypkie")]
        DryGoods = 4,

        [Description("przyprawy")]
        Spices = 5,

        [Description("inne")]
        Other = 6
    }
}