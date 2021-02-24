using Orders.DTO;

namespace Orders.Domain.Stock.Handlers
{
    public interface IStockConfirmedHandler
    {
        void Handle(StockConfirmedDto dto);
    }

    public class StockConfirmedHandler: IStockConfirmedHandler
    {
        public void Handle(StockConfirmedDto dto)
        {
            //Do something to alert the user of the stock shortage but may require a user service
            throw new System.NotImplementedException();
        }
    }
}
