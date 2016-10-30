using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace UnifranChat.Hubs
{
    public interface IUserIdProvider
    {
        string GetUserId(IRequest request);
    }
}