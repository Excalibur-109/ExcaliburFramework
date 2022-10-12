namespace Excalibur
{
    public enum ExcelHandler
    {
        ExcelDataReader_xlsx,
        MiniExcel_xlsx,
        NPOI_xlsx_xls
    }

    public enum ExportFormat
    {
        xml,
        json,
        txt
    }

    public enum SQLType
    {
        Oracle,
        Mysql,
        SqlServer
    }

    public enum ValueGenre
    {
        Sbyte,
        Byte,
        Short,
        Ushort,
        Int,
        Uint,
        Long,
        uLong,
        Bool,
        Float,
        Double,
        Char,
        String,

        Count,
        None
    }
}
