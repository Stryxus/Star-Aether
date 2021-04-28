using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UEESA.RSIScraper
{
    internal interface IRSIScraper
    {
        internal abstract void GetPageContent();
        internal abstract void ParseData();
    }
}
