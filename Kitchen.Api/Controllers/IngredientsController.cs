using System;
using Kitchen.Api.Models.Entities;
using Kitchen.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kitchen.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientsController : ControllerBase
    {
       
        // 1. Deklaracja pola - to tutaj "mieszka" serwis wewnątrz kontrolera
        private readonly IInventoryService _inventoryService;

        // 2. Konstruktor - tutaj .NET "wstrzykuje" serwis
        public IngredientsController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }
        
        [HttpGet]
        public IEnumerable<Ingredient> Get()
        {
            // Zwracamy bezpośrednio to, co przygotował serwis
            return _inventoryService.GetAllIngredients();
        }

        [HttpPost]
        public void Post([FromBody] Ingredient newIngredient)
        {
            // Wywołujemy serwis, aby zapisał nowy składnik
            _inventoryService.AddOrUpdateIngredient(newIngredient);
        }
    }
}