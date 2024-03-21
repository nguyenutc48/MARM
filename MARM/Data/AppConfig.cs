using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MARM.Data
{
    public class AppConfig
    {
        [Column("Id", TypeName = "TEXT")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Required]
        public Guid Id { get; set; }
        [Column("Transmit1", TypeName = "BOOL")]
        [Required]
        public bool Transmit1 { get; set; }
        [Column("Transmit2", TypeName = "BOOL")]
        [Required]
        public bool Transmit2 { get; set; }
        [Column("Transmit3", TypeName = "BOOL")]
        [Required]
        public bool Transmit3 { get; set; }
        [Column("Transmit4", TypeName = "BOOL")]
        [Required]
        public bool Transmit4 { get; set; }
        [Column("TimerInterval", TypeName = "INT")]
        [Required]
        public int TimerInterval { get; set; }
        [Column("Light1Mode", TypeName = "INT")]
        [Required]
        public int Light1Mode { get; set; }
        [Column("Light2Mode", TypeName = "INT")]
        [Required]
        public int Light2Mode { get; set; }
        [Column("Light3Mode", TypeName = "INT")]
        [Required]
        public int Light3Mode { get; set; }
        [Column("Light4Mode", TypeName = "INT")]
        [Required]
        public int Light4Mode { get; set; }
        [Column("Baudrate", TypeName = "INT")]
        [Required]
        public int Baudrate { get; set; }
        [Column("Port", TypeName = "TEXT")]
        [Required]
        public string Port { get; set; } = "";
    }
}
