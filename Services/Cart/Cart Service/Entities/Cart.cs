using Blocks.Domain.Entities;
using System.Xml.Linq;

namespace Cart_Service.Entities
{
    public sealed class Cart : BaseEntity
    {
        public Guid CustomerId { get; private set; }

        public decimal Subtotal { get; private set; }
        public decimal Total { get; private set; }

        private readonly List<CartItem> _items = new();
        public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

        private Cart() { }

        private Cart(Guid customerId)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
        }

        public static Cart Create(Guid customerId)
        {
            return new Cart(customerId);
        }

        public CartItem? FindItem(Guid productId)
        {
            return _items.FirstOrDefault(x => x.ProductId == productId);
        }

        public CartItem? FindItemById(Guid cartItemId)
        {
            return _items.FirstOrDefault(x => x.Id == cartItemId);
        }

        public void AddItem(Guid productId, int quantity, decimal unitPrice)
        {
            var existingItem = FindItem(productId);

            if (existingItem is not null)
            {
                existingItem.IncreaseQuantity(quantity);
            }
            else
            {
                _items.Add(CartItem.Create(Id, productId, quantity, unitPrice));
            }

            RecalculateTotals();
        }

        public void RemoveItem(Guid productId)
        {
            var item = FindItem(productId);
            if (item is not null)
            {
                _items.Remove(item);
                RecalculateTotals();
            }
        }

        public void RemoveItemById(Guid cartItemId)
        {
            var item = FindItemById(cartItemId);
            if (item is not null)
            {
                _items.Remove(item);
                RecalculateTotals();
            }
        }

        public void RecalculateTotals()
        {
            Subtotal = _items.Sum(x => x.LineTotal);
            Total = Subtotal;
        }
    }
}
