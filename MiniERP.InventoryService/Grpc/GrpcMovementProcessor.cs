using MiniERP.InventoryService.Models;
using MiniERP.InventoryService.Protos;

namespace MiniERP.InventoryService.Grpc
{
    public class GrpcMovementProcessor : IMovementProcessor
    {
        public bool Open(InventoryItem inventory, StockMovement sm, out string error)
        {
            error = string.Empty;

            if (sm.MovementType == MovementType.Out)
            {
                if (inventory.Stock.Quantity < sm.Quantity)
                {
                    error = $"Article ID : {inventory.ArticleId} -- OUT : {sm.Quantity} > STOCK : {inventory.Stock.Quantity}";
                    return false;
                }
                inventory.Stock.Quantity -= sm.Quantity;
            }

            sm.MovementStatus = MovementStatus.Open;

            return true;
        }

        public bool Close(List<StockMovement> movements, out string error)
        {
            error = string.Empty;

            foreach(StockMovement movement in movements)
            {
                if(movement.MovementStatus == MovementStatus.Cancelled)
                {
                    error = $"Order type= {movement.RelatedOrderType} : OrderId= {movement.RelatedOrderId} : ArticleId={movement.ArticleId} : Cancelled";
                    return false;
                }
                else if(movement.MovementStatus == MovementStatus.Closed)
                {
                    continue;
                }

                if(movement.MovementType == MovementType.In)
                {
                    movement.Article.Stock.Quantity += movement.Quantity;
                }

                movement.MovementStatus = MovementStatus.Closed;
            }
            return true;
        }

        public bool Cancel(List<StockMovement> movements, out string error)
        {
            error = string.Empty;

            foreach (StockMovement movement in movements)
            {
                if (movement.MovementStatus == MovementStatus.Closed)
                {
                    error = $"Order type= {movement.RelatedOrderType} : OrderId= {movement.RelatedOrderId} : ArticleId={movement.ArticleId} : Closed";
                    return false;
                }
                else if (movement.MovementStatus == MovementStatus.Cancelled)
                {
                    continue;
                }

                if (movement.MovementType == MovementType.Out)
                {
                    movement.Article.Stock.Quantity += movement.Quantity;
                }

                movement.MovementStatus = MovementStatus.Cancelled;
            }
            return true;
        }
    }
}
