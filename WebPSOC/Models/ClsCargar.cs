using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace WebPSOC.Models
{
    public class ClsCargar
    {
        public void CargarArchivos()
        {
            // string pathBackup = RegistroParametros.Cargar("path_ordenes_backup");
            // string pathError = RegistroParametros.Cargar("path_ordenes_error");

            // Dejamos listado de nuevos archivos en variable archivos.

            //string[] archivos = Directory.GetFiles(RegistroParametros.Cargar("path_ordenes_nuevas"));
            string[] listadoarchivos = Directory.GetFiles("D:\\SOC\\Ordenes");

            foreach (string archivo in listadoarchivos)
            {
                ClsOrdenCompra oc = null;

                //D:\\SOC\\Ordenes\\12351123.txt
                //partes[0] D:
                //partes[1] SOC
                //partes[2] Ordenes 
                //partes[3] 12351123.txt //4

                string[] partes = archivo.Split(new char[] { '\\' });
                //devolver ultima posicion
                string nombreCorto = partes[partes.Length - 1];

                try
                {
                    //interpreta cada uno de los archivos y devuelve una clase clsOrdenCompra
                    oc = CargarDesdeEDICL(archivo);
                }
                catch (Exception e)
                {
                    //SPOCLog.Error("Archivo EDICL " + nombreCorto + " no pudo ser leido: " + e.Message);
                    try
                    {
                        File.Move(archivo, "D:\\SOC\\error\\" + nombreCorto);
                    }
                    catch (Exception e2)
                    {
                        //SPOCLog.Error("No fue posible mover archivo con error desde la bandeja de entrada: " + e2.Message);
                    }
                    continue;
                }

                oc.Estado = 0;
                try
                {
                    AgregarOrdenCompra(oc);
                }
                catch (Exception e)
                {
                    //SPOCLog.Error("Archivo " + nombreCorto + " no pudo ser cargado en la BD: " + e.Message);
                    try
                    {
                        File.Move(archivo, "D:\\SOC\\error\\" + nombreCorto);
                    }
                    catch (Exception e2)
                    {
                        // SPOCLog.Error("No fue posible mover archivo con error desde la bandeja de entrada: " + e2.Message);
                    }
                    continue;
                }

                //SPOCLog.Info("Archivo " + nombreCorto + " cargado con exito");

                try
                {
                    File.Move(archivo, "D:\\SOC\\Respaldo\\" + nombreCorto);
                }
                catch (Exception e)
                {
                    //SPOCLog.Error("No fue posible eliminar archivo desde la bandeja de entrada: " + e.Message);
                }
            }

            // RegistroParametros.Grabar("fecha_ultima_carga", DateTime.Now.ToString("dd-MM-yyyy HH:mm"));

            //SPOCLog.Info("Carga de archivos finalizada");
            //return "hola mundo";
        }
        /**
        * Cargar una OC desde archivo con formato EDICL
        */
        private ClsOrdenCompra CargarDesdeEDICL(string archivo)
        {
            ClsOrdenCompra oc = new ClsOrdenCompra();
            string contenido = "";
            TextReader tr = null;

            if (!File.Exists(archivo))
            {
                throw (new Exception("Archivo EDICL no existe"));
            }

            try
            {
                tr = new StreamReader(archivo);
                contenido = tr.ReadToEnd();
            }
            catch
            {
                throw (new Exception("No fue posible leer archivo EDICL"));
            }
            finally
            {
                tr.Close();
                tr.Dispose();
            }

            oc.Lineas = new List<ClsLineaOrdenCompra>();
            contenido = contenido.Replace("\n", "");
            string[] lineas = contenido.Split('\r');

            int datosOK = 0;

            ClsLineaOrdenCompra linea = null;
            int descuentoCargo = 0;
            int totalDescuentos = 0;

            foreach (string l in lineas)
            {
                if (l.Length < 3)
                {
                    continue;
                }

                string etiqueta = l.Substring(0, 3);

                // Código de la cadena

                if (etiqueta == "UNB")
                {
                    //substring(desde, largo)
                    oc.CodigoCadena = l.Substring(3, 13);
                    datosOK++;
                }

                // Número de documento

                if ((etiqueta == "BGM") && (l.Substring(3, 3) == "220"))
                {
                    oc.Numero = l.Substring(6, 21);
                    datosOK++;
                }

                // Fecha de envío y entrega

                if (etiqueta == "DTM")
                {
                    if (l.Substring(3, 3) == "137")
                    {
                        oc.FechaEnvio = new DateTime(int.Parse(l.Substring(6, 4)), int.Parse(l.Substring(10, 2)), int.Parse(l.Substring(12, 2)));
                        datosOK++;
                    }
                    if (l.Substring(3, 3) == "  2")
                    {
                        oc.FechaEntrega = new DateTime(int.Parse(l.Substring(6, 4)), int.Parse(l.Substring(10, 2)), int.Parse(l.Substring(12, 2)));
                        datosOK++;
                    }
                }

                // Código de local

                if ((etiqueta == "LOC") && (l.Substring(3, 3) == "  7"))
                {
                    oc.CodigoLocal = l.Substring(6, 13);
                    datosOK++;
                }

                // Líneas con productos

                if (etiqueta == "LIN")
                {
                    if (linea != null)
                    {
                        oc.Lineas.Add(linea);
                    }

                    totalDescuentos = 0;

                    linea = new ClsLineaOrdenCompra();
                    linea.Numero = oc.Lineas.Count + 1;
                    linea.Descuentos = new decimal[4] { 0.0M, 0.0M, 0.0M, 0.0M };
                    linea.Flete = 0.0M;
                    linea.CodigoEAN = l.Substring(9, 14);
                    if (linea.CodigoEAN.Substring(0, 1) == "0")
                    {
                        linea.CodigoEAN = linea.CodigoEAN.Substring(1);
                    }

                }

                // Descripción del producto

                if ((etiqueta == "IMD") && (l.Substring(3, 3) == "F  "))
                {
                    linea.Descripcion += l.Substring(9);
                }

                // Cantidad pedida

                if ((etiqueta == "QTY") && (l.Substring(3, 3) == " 21"))
                {
                    linea.Cantidad = int.Parse(l.Substring(6, 15));
                }

                // Precio neto línea

                if ((etiqueta == "MOA") && (l.Substring(3, 3) == "203"))
                {
                    linea.PrecioTotal = Convert.ToDecimal(l.Substring(6, 18), System.Globalization.CultureInfo.InvariantCulture);
                }

                // Precio unitario

                if ((etiqueta == "PRI") && (l.Substring(3, 3) == "AAA"))
                {
                    linea.PrecioUnitario = Convert.ToDecimal(l.Substring(6, 15), System.Globalization.CultureInfo.InvariantCulture);
                }

                if (linea != null)
                {

                    // Descuento

                    if ((etiqueta == "ALC") && (l.Substring(3, 3) == "A  "))
                    {
                        descuentoCargo = -1;
                    }

                    // Cargo

                    if ((etiqueta == "ALC") && (l.Substring(3, 3) == "C  "))
                    {
                        descuentoCargo = 1;
                    }

                    if ((etiqueta == "PCD") && (descuentoCargo != 0))
                    {
                        if (l.Substring(3, 3) == "  1")
                        {
                            decimal descuento = Convert.ToDecimal(l.Substring(6, 10), System.Globalization.CultureInfo.InvariantCulture);
                            if ((descuento > 0) && (totalDescuentos < 4))
                            {
                                linea.Descuentos[totalDescuentos++] = descuento;
                            }
                        }

                        descuentoCargo = 0;
                    }

                    if ((etiqueta == "MOA") && ((l.Substring(3, 3) == " 23") || (l.Substring(3, 3) == "204")) && (descuentoCargo != 0))
                    {
                        linea.Flete = Convert.ToDecimal(l.Substring(6, 18), System.Globalization.CultureInfo.InvariantCulture);
                        descuentoCargo = 0;
                    }
                }

                // Monto total OC

                if ((etiqueta == "MOA") && (l.Substring(3, 3) == " 86"))
                {
                    if (linea != null)
                    {
                        oc.Lineas.Add(linea);
                    }
                    oc.TotalOrden = Convert.ToDecimal(l.Substring(6, 18), System.Globalization.CultureInfo.InvariantCulture);
                    datosOK++;
                }

            }

            if (datosOK != 6)
            {
                throw (new Exception("Formato de archivo EDICL es invalido"));
            }

            return oc;
        }

        public void AgregarOrdenCompra(ClsOrdenCompra oc)
        {

            using (SqlConnection con = new SqlConnection(ClsConfig.Instance.GetConnectionString()))
            {
                string sql = "";
                try
                {
                    con.Open();

                    sql = "INSERT INTO orden_compra (codigo_cadena, codigo_local, numero, fecha_envio, fecha_entrega, fecha_procesamiento, estado, total_orden, fecha_modificacion) VALUES (";
                    sql += "'" + oc.CodigoCadena + "',";
                    sql += "'" + oc.CodigoLocal + "',";
                    sql += "'" + oc.Numero + "',";
                    sql += "'" + oc.FechaEnvio.ToString("yyyyMMdd") + "',";
                    sql += "'" + oc.FechaEntrega.ToString("yyyyMMdd") + "',";
                    sql += "null,";
                    sql += "0,";
                    sql += oc.TotalOrden.ToString().Replace(",", ".") + ",";
                    sql += "GETDATE())";

                    SqlCommand cmd = new SqlCommand(sql, con);

                    cmd.ExecuteNonQuery();

                    foreach (ClsLineaOrdenCompra linea in oc.Lineas)
                    {
                        sql = "INSERT INTO linea (numero_orden, linea, codigo_producto, cantidad, precio_unitario, precio_total, descuento_1, descuento_2, descuento_3, descuento_4, flete, descripcion) VALUES (";
                        sql += "'" + oc.Numero + "'";
                        sql += "," + linea.Numero.ToString();
                        sql += ",'" + linea.CodigoEAN + "'";
                        sql += "," + linea.Cantidad.ToString();
                        sql += "," + linea.PrecioUnitario.ToString().Replace(",", ".");
                        sql += "," + linea.PrecioTotal.ToString().Replace(",", ".");
                        sql += "," + linea.Descuentos[0].ToString().Replace(",", ".");
                        sql += "," + linea.Descuentos[1].ToString().Replace(",", ".");
                        sql += "," + linea.Descuentos[2].ToString().Replace(",", ".");
                        sql += "," + linea.Descuentos[3].ToString().Replace(",", ".");
                        sql += "," + linea.Flete.ToString().Replace(",", ".");
                        sql += ",'" + linea.Descripcion.Replace("'", " ") + "')";

                        cmd = new SqlCommand(sql, con);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    throw (e);
                }
            }
        }
    }
}