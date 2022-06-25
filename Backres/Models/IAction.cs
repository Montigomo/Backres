using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backres.Models
{
    internal interface IAction
    {

        bool Run();

        Task<bool> RunAsync();
    }
}
