using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebPSOC.Models
{
    public class ClsOrdenCompra
    {
        public string CodigoCadena { get; set; }
        public int Estado { get; set; }
        //public int IdCadena { get; set; }
        //public string NombreCadena { get; set; }
        public string CodigoLocal { get; set; }
        //public string NombreLocal { get; set; }
        public string Numero { get; set; }
        public DateTime FechaEnvio { get; set; }
        public DateTime FechaEntrega { get; set; }
        //public DateTime? FechaPostdateo { get; set; }
        //public DateTime? FechaProcesamiento { get; set; }
        //public DateTime? FechaModificacion { get; set; }
        //public int Estado { get; set; }
        public decimal TotalOrden { get; set; }
        public decimal TotalOrdenCalculado { get; set; }
        //public decimal TotalTruck { get; set; }
        public bool EstaTraspasada { get; set; }
        public int Error { get; set; }
        public List<ClsLineaOrdenCompra> Lineas { get; set; }
        public bool ValidarPrecio { get; set; }
        public bool ValidarFechaEntrega { get; set; }
        public string Usuario { get; set; }
        //public int IdLocal { get; set; }
        //public int IdTerritorio { get; set; }
        //public bool HayEwoc { get; set; }
        //public string CausaPostdateo { get; set; }
    }

    public class ClsLineaOrdenCompra
    {
        public int Numero { get; set; }
        public string CodigoEAN { get; set; }
        public string Descripcion { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        //public decimal PrecioListaTruck { get; set; }
        public decimal PrecioTotal { get; set; }
        //public decimal PrecioTotalTruck { get; set; }
        public decimal[] Descuentos { get; set; }
        //public decimal[] DescuentosTruck { get; set; }
        public decimal Flete { get; set; }
        public decimal FleteTruck { get; set; }
        public int Estado { get; set; }
        public int IdProducto { get; set; }
    }

}