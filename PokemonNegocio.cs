using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using dominio;

namespace negocio
{
    public class PokemonNegocio
    {
        public List<Pokemon> listar()
        {
            List<Pokemon> lista = new List<Pokemon>();

            SqlConnection conexion = new SqlConnection();
            SqlCommand comando = new SqlCommand();
            SqlDataReader lector;

            try
            {
                conexion.ConnectionString = "server=.\\SQLEXPRESS05; database=POKEDEX_DB; integrated security = true";
                comando.CommandType = System.Data.CommandType.Text;
                comando.CommandText = "Select Numero, Nombre, P.Descripcion, UrlImagen, E.Descripcion AS Tipo, D.Descripcion AS Debilidad, P.IdTipo, P.IdDebilidad, P.Id from POKEMONS P, ELEMENTOS E, ELEMENTOS D where P.IdTipo = E.Id And P.IdDebilidad = D.Id AND P.Activo = 1";
                comando.Connection = conexion;

                conexion.Open();

                lector = comando.ExecuteReader();

                while(lector.Read())
                {
                    Pokemon aux = new Pokemon();

                    aux.Id = (int)lector["Id"];
                    aux.Numero = lector.GetInt32(0);
                    aux.Nombre = (string)lector["Nombre"];
                    aux.Descripcion = (string)lector["Descripcion"];

                    if (!(lector["UrlImagen"] is DBNull)) {
                       
                        aux.UrlImagen = (string)lector["UrlImagen"];
                    }

                    

                    aux.Tipo = new Elemento();      // Creo el objeto primero para instanciar Tipo, sino la referencia es NULL
                    aux.Tipo.Id = (int)lector["IdTipo"];
                    aux.Tipo.Descripcion = (string)lector["Tipo"];

                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)lector["IdDebilidad"];
                    aux.Debilidad.Descripcion = (string)lector["Debilidad"];

                    lista.Add(aux);
                }
                
                conexion.Close();

                return lista;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            
        }

	    public void agregar(Pokemon nuevo)
        {

            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("Insert into POKEMONS (Numero, Nombre, Descripcion, Activo, IdTipo, IdDebilidad, UrlImagen) values (" + nuevo.Numero + ", '" + nuevo.Nombre + "', '" + nuevo.Descripcion + "', 1, @idTipo, @idDebilidad, @UrlImagen)");

                datos.setearParametros("@idTipo", nuevo.Tipo.Id);
                datos.setearParametros("@idDebilidad", nuevo.Debilidad.Id);
                datos.setearParametros("@UrlImagen", nuevo.UrlImagen);

                datos.ejecutarAccion();
            
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

        public void modificar(Pokemon modif)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                
                datos.setearConsulta("update POKEMONS set Numero = @numero, Nombre = @nombre, Descripcion = @desc, UrlImagen = @img, IdTipo = @idTipo, IdDebilidad = @idDebilidad where id = @id");

                datos.setearParametros("@numero", modif.Numero);
                datos.setearParametros("@nombre", modif.Nombre);
                datos.setearParametros("@desc", modif.Descripcion);
                datos.setearParametros("@img", modif.UrlImagen);
                datos.setearParametros("@idTipo", modif.Tipo.Id);
                datos.setearParametros("@idDebilidad", modif.Debilidad.Id);
                datos.setearParametros("@id", modif.Id);

                datos.ejecutarAccion();

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


        public void eliminar(int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();

                datos.setearConsulta("delete from POKEMONS where Id = @id");
                datos.setearParametros("@id", id);
                datos.ejecutarAccion();

            }
            catch (Exception ex )
            {

                throw ex;
            }
        }

        public void eliminarLogico(int id)
        {
            try
            {
                AccesoDatos datos = new AccesoDatos();
                datos.setearConsulta("update POKEMONS set Activo = 0 where Id = @id");      // Se escribe '@id' porque el valor hay que recibirlo por parámetro
                datos.setearParametros("@id", id);
                datos.ejecutarAccion();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<Pokemon> filtrar(string campo, string criterio, string filtro)      // este método me va a devolver una lista de pokemons
        {
            List<Pokemon> lista = new List<Pokemon>();      // muy parecido al método listar de esta misma clase
            AccesoDatos datos = new AccesoDatos();          // hay que hacer la conexión a la DB
            try
            {
                string consulta = "Select Numero, Nombre, P.Descripcion, UrlImagen, E.Descripcion AS Tipo, D.Descripcion AS Debilidad, P.IdTipo, P.IdDebilidad, P.Id from POKEMONS P, ELEMENTOS E, ELEMENTOS D where P.IdTipo = E.Id And P.IdDebilidad = D.Id AND P.Activo = 1 And ";

                if (campo == "Número")
                {
                    switch (criterio)
                    {
                        case "Mayor a":
                            consulta += "Numero > " + filtro;
                            break;

                        case "Menor a":
                            consulta += "Numero < " + filtro;
                            break;

                        default:
                            consulta += "Numero = " + filtro;
                            break;
                    }
                } 
                else if (campo == "Nombre")
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "Nombre like ' " + filtro + "%' ";
                            break;

                        case "Termina con":
                            consulta += "Nombre like '%" + filtro + " ' ";
                            break;

                        default:
                            consulta += "Nombre like '%" + filtro + "%' ";
                            break;
                    }
                } 
                else
                {
                    switch (criterio)
                    {
                        case "Comienza con":
                            consulta += "P.Descripcion like '" + filtro + "%' ";
                            break;

                        case "Termina con":
                            consulta += "P.Descripcion like '%" + filtro + " ' ";
                            break;

                        default:
                            consulta += "P.Descripcion like '%" + filtro + "%' ";
                            break;
                    }
                }

                datos.setearConsulta(consulta);
                datos.ejecutarLectura();


                while (datos.Lector.Read())
                {
                    Pokemon aux = new Pokemon();

                    aux.Id = (int)datos.Lector["Id"];
                    aux.Numero = datos.Lector.GetInt32(0);
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];

                    if (!(datos.Lector["UrlImagen"] is DBNull))
                    {

                        aux.UrlImagen = (string)datos.Lector["UrlImagen"];
                    }



                    aux.Tipo = new Elemento();      // Creo el objeto primero para instanciar Tipo, sino la referencia es NULL
                    aux.Tipo.Id = (int)datos.Lector["IdTipo"];
                    aux.Tipo.Descripcion = (string)datos.Lector["Tipo"];

                    aux.Debilidad = new Elemento();
                    aux.Debilidad.Id = (int)datos.Lector["IdDebilidad"];
                    aux.Debilidad.Descripcion = (string)datos.Lector["Debilidad"];

                    lista.Add(aux);
                }


                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
