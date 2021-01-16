namespace XmlParser.Core.DataContext
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ActiveChannel")]
    public partial class ActiveChannel
    {
        [Key]
        public long Srno { get; set; }

        public long SourceId { get; set; }

        [Required]
        public string ChannelName { get; set; }

        public bool IsActive { get; set; }

        public bool IsManuallyAdded { get; set; }

        public virtual SourceURL SourceURL { get; set; }
    }
}
