using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace civica_service.Services
{
    public interface ICouncilTaxService
    {
        string GetCouncilTaxTransactions(string personReference, string accountReference, int year);
    }
}
