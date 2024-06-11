using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanejamentoDeViagem
{
    [Table("itinerario")]
    public class Itinerario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ViagemId { get; set; }
        public string Titulo { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan Hora { get; set; }
        public string Local { get; set; }
    }

}
