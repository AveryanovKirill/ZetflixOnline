using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer.Services
{
    internal interface IMailService
    {
        Task SendAsync();
    }
}
