using System.Collections.Generic;

namespace Services 
{
    public interface ICartService 
    {
        double Total();
        IEnumerable<ICartItem> Items();
    }
}