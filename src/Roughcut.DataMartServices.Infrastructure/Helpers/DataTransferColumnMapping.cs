namespace Roughcut.DataMartServices.Infrastructure.Helpers
{
    public class DataTransferColumnMapping
    {
        public string SourceColumnName { get; set; }
        public string TargetColumnName { get; set; }
        public string ColumnSqlTypeName { get; set; }
        public string TransformationMethod { get; set; }
        public string ExtTableName { get; set; }
    }
}
