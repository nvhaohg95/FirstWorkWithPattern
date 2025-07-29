using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FirstWorkWithPattern.Base
{
    public abstract class BaseController<T> : ControllerBase
    {
        protected readonly ILogService<T> _log;
        protected BaseController(ILogService<T> log)
        {
            _log = log;
        }
    }
}
