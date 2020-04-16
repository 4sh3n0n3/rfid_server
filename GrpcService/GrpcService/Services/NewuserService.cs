using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcService
{
    public class NewuserService : Newuser.NewuserBase
    {
        private readonly ILogger<NewuserService> _logger;
        public NewuserService(ILogger<NewuserService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply1> SayHello(HelloRequest1 request, ServerCallContext context)
        {
           // Greeter.GreeterBase.SayHello(HelloRequest request, ServerCallContext context);
            return Task.FromResult(new HelloReply1
            {
                Message = "Hello " + request.Name
            });
        }
    }
}
