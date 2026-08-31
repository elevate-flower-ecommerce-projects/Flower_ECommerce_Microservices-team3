using Blocks.Domain.Entities;

namespace Cart_Service.Entities
{
    public sealed class CartItem : BaseEntity
    {
        public Guid CartId { get; private set; }
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public decimal LineTotal => UnitPrice * Quantity;

        public Cart Cart { get; private set; } = null!;

        private CartItem() { }

        private CartItem(Guid cartId, Guid productId, int quantity, decimal unitPrice)
        {
            Id = Guid.NewGuid();
            CartId = cartId;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public static CartItem Create(Guid cartId, Guid productId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative.");

            return new CartItem(cartId, productId, quantity, unitPrice);
        }

        public void IncreaseQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            Quantity += quantity;
        }

        public void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            Quantity = newQuantity;
        }
    }
}
