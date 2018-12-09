using EFDataCoreDomain.Contexts;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace EFDataCoreDomain.Services
{
    public class BaseService
    {
        protected readonly QGDDbContexts dbContexts;

        public BaseService()
        {
            if (dbContexts == null)
                dbContexts = new QGDDbContexts();
        }

    }
}
