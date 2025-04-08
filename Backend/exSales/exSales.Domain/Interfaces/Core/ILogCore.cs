using System;
using exSales.Domain.Impl.Core;
using Microsoft.Extensions.Logging;

namespace exSales.Domain.Interfaces.Core
{
    public interface ILogCore
    {
        void Log(string message, Levels level);
    }
}
