using NETSandbox.Entities;
using System;

namespace NETSandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            Customer customer = new()
            {
                Name = "Juozas",
                LastName = "Juozaitis",
                IsIndividual = true,
                Address = "Laisves al. Kaunas",
                Country = "LTU"
            };

            ServiceProvider serviceProvider = new()
            {
                Name = "Mechanikas",
                Country = "LTU",
                VATPayer = true
            };
        }
    }
}
