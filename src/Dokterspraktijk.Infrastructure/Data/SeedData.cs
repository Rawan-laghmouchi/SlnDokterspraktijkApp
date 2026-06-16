using Dokterspraktijk.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Dokterspraktijk.WebUI.Data
{
    public static class SeedData
    {
        public static void Initialize(UserManager<ApplicationUser> userManager)
        {
            MaakGebruikerAanAlsNietBestaat(
                userManager,
                "rawan.laghmouchi@gmail.be",
                "Admin123!",
                "Rawan",
                "Laghmouchi",
                new List<Claim>
                {
                    new Claim("IsAdmin", "true")
                });

            MaakGebruikerAanAlsNietBestaat(
                userManager,
                "timmermans@dokterspraktijk.be",
                "Dokter123!",
                "Dokter",
                "Timmermans",
                new List<Claim>
                {
                    new Claim("IsDokter", "true"),
                    new Claim("DokterNaam", "Timmermans")
                });

            MaakGebruikerAanAlsNietBestaat(
                userManager,
                "brancaert@dokterspraktijk.be",
                "Dokter123!",
                "Dokter",
                "Brancaert",
                new List<Claim>
                {
                    new Claim("IsDokter", "true"),
                    new Claim("DokterNaam", "Brancaert")
                });

            MaakGebruikerAanAlsNietBestaat(
                userManager,
                "olali@dokterspraktijk.be",
                "Dokter123!",
                "Dokter",
                "Olali",
                new List<Claim>
                {
                    new Claim("IsDokter", "true"),
                    new Claim("DokterNaam", "Olali")
                });

            MaakGebruikerAanAlsNietBestaat(
                userManager,
                "Okondo@dokterspraktijk.be",
                "Dokter123!",
                "Dokter",
                "Okondo",
                new List<Claim>
                {
                    new Claim("IsDokter", "true"),
                    new Claim("DokterNaam", "Okondo")
                });
            MaakGebruikerAanAlsNietBestaat(
                userManager,
                "hans.vandenbogaerde@gmail.be",
                "Patient123!",
                "Hans",
                "Vandenbogaerde",
                new List<Claim>());
        }

        private static void MaakGebruikerAanAlsNietBestaat(
            UserManager<ApplicationUser> userManager,
            string email,
            string wachtwoord,
            string voornaam,
            string achternaam,
            List<Claim> claims)
        {
            ApplicationUser? bestaandeGebruiker = userManager.FindByEmailAsync(email).Result;

            if (bestaandeGebruiker == null)
            {
                ApplicationUser gebruiker = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    Voornaam = voornaam,
                    Achternaam = achternaam,
                    EmailConfirmed = true,
                    LockoutEnabled = true
                };

                IdentityResult result = userManager.CreateAsync(gebruiker, wachtwoord).Result;

                if (result.Succeeded)
                {
                    userManager.AddClaimsAsync(gebruiker, claims).Wait();
                }

                return;
            }

            IList<Claim> bestaandeClaims = userManager.GetClaimsAsync(bestaandeGebruiker).Result;

            foreach (Claim claim in claims)
            {
                bool claimBestaatAl = bestaandeClaims.Any(bestaandeClaim =>
                    bestaandeClaim.Type == claim.Type &&
                    bestaandeClaim.Value == claim.Value);

                if (!claimBestaatAl)
                {
                    userManager.AddClaimAsync(bestaandeGebruiker, claim).Wait();
                }
            }
        }
    }
}