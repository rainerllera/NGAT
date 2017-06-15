using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NGAT.Business.Domain.Base;
using NGAT.Business.Domain.Core;

namespace NGAT.DataAccess.Implementation
{
    public class ObjectContext : DbContext 
    {
        public DbSet<Arc> Arcs { get; set; }

        public DbSet<Node> Nodes { get; set; }

        public DbSet<Graph> Graps { get; set; }
    }
}
