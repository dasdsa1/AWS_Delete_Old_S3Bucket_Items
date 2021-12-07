
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNet.Identity.EntityFramework;


namespace DFGE_lambda
{
    class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Customer> Customer { get; set; }

        public ApplicationDbContext(DbContextOptions<DbContext> options)         
        {

        }
    }
}
