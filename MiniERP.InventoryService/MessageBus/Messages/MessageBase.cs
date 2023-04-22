using MediatR;

namespace MiniERP.InventoryService.MessageBus.Messages
{
    public class MessageBase : IRequest
    {
        public int Id { get; set; }
    }
}
