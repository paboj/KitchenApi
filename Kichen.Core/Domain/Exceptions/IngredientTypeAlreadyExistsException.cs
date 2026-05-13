using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kitchen.Core.Domain.Exceptions
{
    public sealed class ProductDefinitionAlreadyExistsException : KitchenApiException
    {
        public ProductDefinitionAlreadyExistsException() : base("Product already defined.") { }
    }
}
