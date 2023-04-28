using MediatR;

namespace MiniERP.InventoryService.Commands
{
    public class InvalidateCacheCommand : IRequest
    {
        public int Id { get; }
        public InvalidateCacheCommand(int id)
        {
            Id = id;
        }
    }
}
