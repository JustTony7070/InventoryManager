using InventoryManager.Interfaces;

namespace InventoryManager.Classes
{
    class Product(int Id, string Name, int Quantity, decimal Price, string Code) : IData
    {
        public int Id { get; set; } = Id;
        public string Name { get; set; } = Name;
        public int Quantity { get; set; } = Quantity;
        public decimal Price { get; set; } = Price;
        public string Code { get; set; } = Code;
    }
}
