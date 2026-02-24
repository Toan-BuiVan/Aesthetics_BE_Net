using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aesthetics.Data.AestheticsInterfaces.ICommonService
{
    public interface ICommonService
    {
		Task<string> BaseProcessingFunction64(string? servicesImage);
	}
}
