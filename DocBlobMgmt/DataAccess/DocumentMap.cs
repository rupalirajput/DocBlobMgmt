using System.Data.Entity.ModelConfiguration;

namespace WebApplication.DataAccess
{
    public class DocumentMap : EntityTypeConfiguration<Document>
    {
        public DocumentMap()
        {
            ToTable("Document");

            Property(p => p.Id).IsRequired();
            Property(p => p.PropertyId).IsRequired();
            Property(p => p.DocType).IsRequired();
            Property(p => p.FileName).IsRequired();
            Property(p => p.DocBlob).IsRequired();
        }
    }
}