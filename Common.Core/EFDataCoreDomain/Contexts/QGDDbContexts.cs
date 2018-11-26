using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
namespace EFDataCoreDomain.Contexts
{
    public class QGDDbContexts:DbContext
    {
        public QGDDbContexts(DbContextOptions<QGDDbContexts> options):base(options)
        {
        }


    }
}
