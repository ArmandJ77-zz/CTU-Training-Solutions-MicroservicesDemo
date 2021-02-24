using Orders.DTO;

namespace Orders.Domain.Stock.Handlers
{
    public interface IStockShortageHandler
    {
        void Handle(StockShortageDto dto);
    }

    public class StockShortageHandler : IStockShortageHandler
    {
        public void Handle(StockShortageDto dto)
        {
            //Do something to alert the user of the stock shortage but may require a user service
            throw new System.NotImplementedException();
        }
    }
}
