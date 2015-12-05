namespace XmlParser.DataContext
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class EPGDataModel : DbContext
    {
        public EPGDataModel()
            : base("name=EPGDataModel")
        {
        }

        public virtual DbSet<ActiveChannel> ActiveChannels { get; set; }
        public virtual DbSet<SourceURL> SourceURLs { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<XmlImport> XmlImports { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SourceURL>()
                .HasMany(e => e.ActiveChannels)
                .WithRequired(e => e.SourceURL)
                .HasForeignKey(e => e.SourceId)
                .WillCascadeOnDelete(false);
        }
    }
}
