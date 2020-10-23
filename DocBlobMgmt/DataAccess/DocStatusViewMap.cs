using System.Data.Entity.ModelConfiguration;

namespace WebApplication.DataAccess
{
    public class DocStatusViewMap : EntityTypeConfiguration<DocStatusView>
    {
        public DocStatusViewMap()
        {
            ToTable("DocStatusView");

            Property(p => p.PropertyId).IsRequired();
            Property(p => p.Agreement).IsRequired();
            Property(p => p.Appraisal).IsRequired();
            Property(p => p.SiteMap).IsRequired();
            Property(p => p.Resume).IsRequired();
            Property(p => p.Paperwork).IsRequired();
        }
    }
}