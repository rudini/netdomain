//-------------------------------------------------------------------------------
// <copyright file="TestDbContext.cs" company="bbv Software Services AG">
//   Copyright (c) 2010 Roger Rudin, bbv Software Services AG
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//------------------------------------------------------------------------------

namespace netdomain.LinqToDbContext.Test.BusinessObjects
{
    using System.Data.Entity;

    public class TestDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestDbContext"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public TestDbContext(string connectionString)
            : base(connectionString)
        {
        }

        public DbSet<Adresse> Adresse { get; set; }

        public DbSet<Person> Person { get; set; }

        public DbSet<AdresseDetail> AdresseDetail { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().Map(m => m.ToTable("Person"))
                .Property(m => m.Version).IsRowVersion();   //.HasColumnType("timestamp").IsConcurrencyToken();

            modelBuilder.Entity<Adresse>().Map(m => m.ToTable("Adresse"))
                .HasOptional(a => a.Person)
                .WithMany(p => p.Adressliste)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Adresse>().Property(m => m.Version).IsRowVersion(); //.HasColumnType("timestamp").IsConcurrencyToken();

            //modelBuilder.Entity<Person>()
            //    .Map(m => m.ToTable("Person"))
            //    .HasOptional(p => p.Adressliste).WithMany().Map(x => x.MapKey("PersonID"));
            //modelBuilder.Entity<Adresse>().Map(m => m.ToTable("Adresse")).Property(a => a.Id);
            //modelBuilder.Entity<AdresseDetail>().Map(m => m.ToTable("AdresseDetail"));
        }
    }
}