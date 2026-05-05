using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitchen.Core.Domain.Exceptions
{
    public sealed class IngredientAlreadyExistsException : KitchenApiException
    {
        public IngredientAlreadyExistsException() : base("Ingredient already exists.") { }
    }
}
