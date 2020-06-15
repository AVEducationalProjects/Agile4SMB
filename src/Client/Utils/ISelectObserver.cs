using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Agile4SMB.Client.Utils
{
    public interface ISelectObserver<TItem> where TItem : class
    {
        TItem Item { get;}
        void Select(TItem item);
    }
}
