using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using P3AddNewFunctionalityDotNetCore.Data;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public abstract class DataBaseTest
    {
        public DbContextOptions<P3Referential> builder;
        public P3Referential context;
        public DataBaseTest()
        {
            builder = new DbContextOptionsBuilder<P3Referential>().UseSqlServer($"Server=.\\SQLEXPRESS;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;MultipleActiveResultSets=true").Options;
            context = new P3Referential(builder);

            if (context != null)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }
    }
}
