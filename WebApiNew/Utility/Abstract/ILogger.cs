using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiNew.Utility.Abstract
{
    public interface ILogger
    {
        void Debug(string text);
        void Error(string text);
        void Error(Exception e);
        void Warning(string text);
        void Trace(string text);
        void Fatal(string text);
        void Info(string text);
    }
}
