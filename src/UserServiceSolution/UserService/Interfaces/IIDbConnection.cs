using System.Data;

namespace visa_direct.Interfaces
{
    public interface IIDbConnection
    {
        IDbConnection CreateConnectionTSql();
        IDbConnection CreateConnectionPgSql();
    }
}
