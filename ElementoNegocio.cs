using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using dominio;

namespace negocio
{
    public class ElementoNegocio
    {
        public List<Elemento> listar()
        {
            List<Elemento> lista = new List<Elemento>();

            AccesoDatos datos = new AccesoDatos();      // Nace un objeto que tiene una conexión, comando y lector
                                                        // El comando y conexión ya tienen instancias, y una cadena de conexión configurada

            try
            {
                // Setear la consulta que yo quiero realizar: 

                datos.setearConsulta("Select Id, Descripcion from ELEMENTOS");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Elemento aux = new Elemento();

                    aux.Id = (int)datos.Lector["Id"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];

                    lista.Add(aux);
                }

                return lista;
            }

            catch (Exception ex)
            {

                throw ex;
            } 

            finally
            {
                datos.cerrarConexion();
            }


        }
    }
}
