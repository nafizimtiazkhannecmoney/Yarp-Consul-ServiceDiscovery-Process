
using System.Data;
using Microsoft.Data.SqlClient;

/**
 * @Author: Masud Ahmed
 * @Since: April 2024
 */
namespace visa_direct.Interfaces
{
    public interface ITransactionService
    {
        public String? Process(string json, string spName, string provider);
    }

}
