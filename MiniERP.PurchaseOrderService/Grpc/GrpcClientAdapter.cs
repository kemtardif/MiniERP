﻿using MiniERP.PurchaseOrderService.Grpc.Protos;

namespace MiniERP.PurchaseOrderService.Grpc
{
    public class GrpcClientAdapter : IGrpcClientAdapter
    {
        private readonly GrpcInventory.GrpcInventoryClient _grpcClient;

        public GrpcClientAdapter(GrpcInventory.GrpcInventoryClient grpcClient)
        {
            _grpcClient = grpcClient;
        }

        public StockResponse GetStockByArticleId(StockRequest request)
        {
            return _grpcClient.GetStockByArticleId(request);
        }

        public StockChangedResponse StockChanged(StockChangedRequest request)
        {
            return _grpcClient.StockChanged(request);
        }
    }
}