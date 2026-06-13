using Dokterspraktijk.Domain.Entities;
using Dokterspraktijk.Domain.Enums;
using Dokterspraktijk.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dokterspraktijk.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Dokter> Dokters { get; set; }
        public DbSet<Patient> Patienten { get; set; }
        public DbSet<Tijdslot> Tijdsloten { get; set; }
        public DbSet<Afspraak> Afspraken { get; set; }
        public DbSet<AfspraakCategorie> AfspraakCategorieen { get; set; }
        public DbSet<Doktersattest> Doktersattesten { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Dokter>().HasKey(dokter => dokter.Id);
            modelBuilder.Entity<Patient>().HasKey(patient => patient.Id);
            modelBuilder.Entity<Tijdslot>().HasKey(tijdslot => tijdslot.Id);
            modelBuilder.Entity<Afspraak>().HasKey(afspraak => afspraak.Id);
            modelBuilder.Entity<Doktersattest>().HasKey(doktersattest => doktersattest.Id);
            modelBuilder.Entity<AfspraakCategorie>().HasKey(afspraakCategorie => afspraakCategorie.Id);

            modelBuilder.Entity<Dokter>()
                .Property(dokter => dokter.Naam)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Dokter>()
                .Property(dokter => dokter.Specialisatie)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Patient>()
                .Property(patient => patient.Voornaam)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Patient>()
                .Property(patient => patient.Achternaam)
                .IsRequired()
                .HasMaxLength(100);
            modelBuilder.Entity<Patient>()
                .Property(patient => patient.Email)
                .IsRequired()
                .HasMaxLength(150);

            modelBuilder.Entity<Patient>()
                .Property(patient => patient.Telefoonnummer)
                .IsRequired()
                .HasMaxLength(30);

            modelBuilder.Entity<Patient>()
                .Property(patient => patient.Rijksregisternummer)
                .HasMaxLength(20);
            modelBuilder.Entity<Afspraak>()
                .Property(afspraak => afspraak.Reden)
                .IsRequired()
                .HasMaxLength(250);

            modelBuilder.Entity<Afspraak>()
                .Property(afspraak => afspraak.FotoBestandsnaam)
                .HasMaxLength(255);

            modelBuilder.Entity<Tijdslot>()
                .Property(tijdslot => tijdslot.Status)
                .HasConversion<int>();

            modelBuilder.Entity<Afspraak>()
                .Property(afspraak => afspraak.Status)
                .HasConversion<int>();

            modelBuilder.Entity<AfspraakCategorie>().HasData(
                new AfspraakCategorie { Id = 1, Naam = "Consultatie" }
            );

            modelBuilder.Entity<Dokter>().HasData(
                new Dokter(1, "Timmermans", "Huisarts"),
                new Dokter(2, "Brancaert", "Huisarts")
            );
            modelBuilder.Entity<Patient>().HasData(
                new Patient
                {
                    Id = 1,
                    Voornaam = "Rawan",
                    Achternaam = "Laghmouchi",
                    Email = "rawan.laghmouchi@gmail.be",
                    Telefoonnummer = "0470123456",
                    Rijksregisternummer = "00.00.00-000.00"
                },
                new Patient
                {
                    Id = 2,
                    Voornaam = "Hans",
                    Achternaam = "Vandenbogaerde",
                    Email = "Hans.Vandenbogaerde@gmail.be",
                    Telefoonnummer = "0470654321",
                    Rijksregisternummer = "11.11.11-111.11"
                });

            modelBuilder.Entity<Tijdslot>().HasData(
                new Tijdslot(
                    1,
                    1,
                    new DateOnly(2026, 5, 15),
                    new TimeOnly(10, 30),
                    TijdslotStatus.Beschikbaar),

                new Tijdslot(
                    2,
                    1,
                    new DateOnly(2026, 5, 15),
                    new TimeOnly(9, 30),
                    TijdslotStatus.NietBeschikbaar)
            );
        }
    }
}
