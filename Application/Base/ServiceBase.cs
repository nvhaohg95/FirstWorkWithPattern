using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Base
{
    public abstract class ServiceBase
    {
        public IUnitOfWork _unitOfWork;
    }
}
