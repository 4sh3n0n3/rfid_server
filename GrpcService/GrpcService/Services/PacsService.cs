using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace GrpcService
{
    public class PacsService : PACS.PACSBase
    {
        private readonly ILogger<PacsService> _logger;
        public PacsService(ILogger<PacsService> logger)
        {
            _logger = logger;
        }

        public override Task<NewUserReply> CreateUser(NewUserRequest request, ServerCallContext context)
        {
            // Greeter.GreeterBase.SayHello(HelloRequest request, ServerCallContext context);
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();
            scope.SetVariable("text", request);
            engine.ExecuteFile("add_allowed_script.py", scope);
            dynamic result = scope.GetVariable("response");
            return Task.FromResult(new NewUserReply
            {
                Newuserresponse = result
            });
        }

        public override Task<RewriteUserReply> RewriteUser(RewriteUserRequest request, ServerCallContext context)
        {
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();
            scope.SetVariable("text", request);
            engine.ExecuteFile("write_to_token_scriptt.py", scope);
            dynamic result = scope.GetVariable("response");
            // Greeter.GreeterBase.SayHello(HelloRequest request, ServerCallContext context);
            return Task.FromResult(new RewriteUserReply
            {
                Rewriteresponse = result
            });
        }

        public override Task<DeleteUserReply> DeleteUser(DeleteUserRequest request, ServerCallContext context)
        {
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();
            scope.SetVariable("id", request);
            engine.ExecuteFile("delete_from_allowed_script.py", scope);
            dynamic result = scope.GetVariable("response");
            // Greeter.GreeterBase.SayHello(HelloRequest request, ServerCallContext context);
            return Task.FromResult(new DeleteUserReply
            {
                Deleteresponse = result
            });
        }

        //public override Task<ShowHistoryReply> ShowHistory(ShowHistoryRequest request, ServerCallContext context)
        //{
        //    // Greeter.GreeterBase.SayHello(HelloRequest request, ServerCallContext context);
        //    return Task.FromResult(new ShowHistoryReply
        //    {
        //         Showhistoryid = " ",
        //         Showhistorytext = " ",
        //         Date = " ",
        //         Allowed = true
        //    });
        //}

        public override async Task ShowHistory(ShowHistoryRequest request, IServerStreamWriter<ShowHistoryReply> responseStream, ServerCallContext context)
        {
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();
            scope.SetVariable("date", request);
            engine.ExecuteFile("get_history_script.py", scope);
            dynamic result = scope.GetVariable("data");

            foreach (var z in result)
            {
                await responseStream.WriteAsync(new ShowHistoryReply { Showhistoryid = z[0], Showhistorytext = z[1], Date = z[2], Allowed = z[3] });
                //await Task.Delay(500);
            }
        }

        public override async Task ShowAllowed(ShowAllowedRequest request, IServerStreamWriter<ShowAllowedReply> responseStream, ServerCallContext context)
        {
            //    List<ShowAllowedReply> test1 = new List<ShowAllowedReply>();
            //    await responseStream.WriteAsync(new ShowAllowedReply { Showallowedresponseid = "Вася", Fullname = "0" });
            //    for (int i=0; i<20; i++)
            //    {
            //        test1.Add(new ShowAllowedReply { Fullname = "Вася", Showallowedresponseid = i.ToString() });
            //    }
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();
            //scope.SetVariable("date", request);
            engine.ExecuteFile("get_allowed_script.py", scope);
            dynamic result = scope.GetVariable("response");

            foreach (var z in result)
            {
                await responseStream.WriteAsync(new ShowAllowedReply { Showallowedresponseid = z[0], Fullname = z[1]});
                //await Task.Delay(500);
            }
            //await responseStream.WriteAsync(
            //  new GetOrderStatusResponse { Status = "Created" });
            //await Task.Delay(500);
            //await responseStream.WriteAsync(
            //  new GetOrderStatusResponse { Status = "Validated" });
            //await Task.Delay(1000);
            //await responseStream.WriteAsync(
            //  new GetOrderStatusResponse { Status = "Dispatched" });
        }
    }
}
