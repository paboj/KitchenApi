using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitchen.Core.Domain.Exceptions
{
    public sealed class IngredientTypeAlreadyExistsException : KitchenApiException
    {
        public IngredientTypeAlreadyExistsException() : base("Ingredient type already exists.") { }
    }
}
