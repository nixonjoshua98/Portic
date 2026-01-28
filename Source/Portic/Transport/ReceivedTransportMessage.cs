using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Portic.Transport
{
    public sealed record ReceivedTransportMessage(
        string MessageId,
        string EndpointName,
        string MessageName
    );
}
