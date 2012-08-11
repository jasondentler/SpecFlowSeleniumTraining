using System;
using System.Collections.Generic;

namespace Specs.Infrastructure
{
    public interface IInfrastructure : IDisposable
    {

        void Start();
        void Stop();
        void Reset();

    }
}
