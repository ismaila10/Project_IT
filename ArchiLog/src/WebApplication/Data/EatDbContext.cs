using APILibrary.Core.IdentityUserModel;
using APILibrary.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Data
{

    //classe principale d'accès aux données
    public class EatDbContext : IdentityDbContext<User,Role,int>
    {
        public static readonly ILoggerFactory SqlLogger = LoggerFactory.Create(builder => builder.AddConsole());

        //constructeur qui appel le constructeur parent 
        public EatDbContext(DbContextOptions options) : base(options)
        {
            //super(options); JAVA
        }

        public DbSet<Pizza> Pizzas { get; set; }

        public DbSet<Customer> Customers { get; set; }
    }
}
