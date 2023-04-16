namespace MiniERP.InventoryService.MessageBus.Messages
{
    public class UndefinedMessage : MessageBase
    {
        public string MessageType => "Undefined";
        public byte[] Body => Array.Empty<byte>();
    }
}
