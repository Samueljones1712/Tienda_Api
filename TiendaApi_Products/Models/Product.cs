using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;

namespace TiendaApi.Models
{
    public class Product
    {
        [Key]
        public int idProduct { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        [Column(TypeName = "money")]
        public decimal price { get; set; }
    }
}
