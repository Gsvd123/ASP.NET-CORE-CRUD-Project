﻿using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUD_Test
{
   public class CustomWebApplicatonFactory:WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.UseEnvironment("Test");

            builder.ConfigureServices(services =>
            {
                var descripter= services.SingleOrDefault(temp=>temp.ServiceType==typeof(DbContextOptions<ApplicationDbContext>));

                if (descripter != null)
                {
                   services.Remove(descripter); 
                }

                services.AddDbContext<ApplicationDbContext>(Options =>
                {
                    Options.UseInMemoryDatabase("DatabaseForTesting");
                });

            });
        }
    }
}
