using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Entities
{
    public  class ApplicationDbContext:DbContext
    {
       
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public virtual DbSet<Person> Persons { get; set; }
       public virtual  DbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //Seed country list

            string countriesstring= System.IO.File.ReadAllText("countries.json");

             List<Country> countryList=System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesstring);


            foreach(Country country in countryList)
            {
                modelBuilder.Entity<Country>().HasData(country);
            }

            //seed person list
            string personsstring = System.IO.File.ReadAllText("persons.json");

            List<Person> PersonList=System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsstring);

            foreach(Person person in PersonList)
            {
                modelBuilder.Entity<Person>().HasData(person);
            }


            //Fluent API

            modelBuilder.Entity<Person>().Property(temp => temp.TIN)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("varchar(8)")
                .HasDefaultValue("ABCD12345");

            //Set Unique

            //modelBuilder.Entity<Person>().HasIndex(temp => temp.TIN).IsUnique();

           // modelBuilder.Entity<Person>().HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber]) = 8");
        }

        public List<Person> sp_GetAllPersons()
        {
            return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
        }

        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] sqlParameters = new SqlParameter[]
            {
                new SqlParameter(@"PersonID",person.PersonID),
                new SqlParameter(@"PersonName",person.PersonName),
                new SqlParameter(@"Email",person.Email),
                new SqlParameter(@"DateOfBirth",person.DateOfBirth),
                new SqlParameter(@"Gender",person.Gender),
                new SqlParameter(@"CountryID",person.CountryID),
                new SqlParameter(@"Address",person.Address),
                new SqlParameter(@"ReceiveNewsLetters",person.ReceiveNewsLetters)
            };

            return Database.ExecuteSqlRaw("EXECUTE [dbo].[InsertPerson] @PersonID,@PersonName,@Email,@DateOfBirth,@Gender,@CountryID,@Address,@ReceiveNewsLetters", sqlParameters);
        }


    }
}
