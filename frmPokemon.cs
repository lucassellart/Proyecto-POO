using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dominio;
using negocio;

namespace winform_app
{
    public partial class frmPokemons : Form
    {
        private List<Pokemon> listaPokemon;

        public frmPokemons()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cargar();

            // Cargar los desplegables para el filtro avanzado (contra la DB):
            cboCampo.Items.Add("Número");
            cboCampo.Items.Add("Nombre");
            cboCampo.Items.Add("Descripción");
        }

        private void cargar()       // Cargar y actualizar la tabla con los pokemons
        {
            PokemonNegocio negocio = new PokemonNegocio();

            try
            {
                listaPokemon = negocio.listar();
                dgvPokemons.DataSource = listaPokemon;
                ocultarColumna();
                cargarImagen(listaPokemon[0].UrlImagen);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void ocultarColumna()
        {
            dgvPokemons.Columns["Id"].Visible = false;
            dgvPokemons.Columns["UrlImagen"].Visible = false;
        }



        private void dgvPokemons_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPokemons.CurrentRow != null)     // Se hace esta validación para poder seleccionar otro pokemon y que no se rompa si la fila llega a ser nula
            {
                Pokemon seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;

                cargarImagen(seleccionado.UrlImagen);
            }
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pbPokemon.Load(imagen);
            }
            catch (Exception ex)
            {
                pbPokemon.Load("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT9cSGzVkaZvJD5722MU5A-JJt_T5JMZzotcw&s");
            }

            
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            frmAltaPokemon alta = new frmAltaPokemon();
            alta.ShowDialog();

            cargar();       // Actualizar la grilla una vez que el pokemon se cargó correctamente
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            Pokemon seleccionado;       // este es el pokemon que voy a pasar por parámetro
            seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;       // así se selecciona un pokemon de la grilla 

            frmAltaPokemon modificar = new frmAltaPokemon(seleccionado);
            modificar.ShowDialog();

            cargar();
        }

        private void btnEliminarFisico_Click(object sender, EventArgs e)
        {
            eliminar();
        }

        private void btnEliminarLogico_Click(object sender, EventArgs e)
        {
            eliminar(true);
        }


        private void eliminar(bool logico = false)      // Recibe por parámetro un valor opcional
        {
            // La funcionalidad para eliminar va a estar en la clase PokemonNegocio

            PokemonNegocio negocio = new PokemonNegocio();

            Pokemon seleccionado;

            try
            {
                DialogResult respuesta = MessageBox.Show("¿De verdad querés eliminarlo?", "Eliminando", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (respuesta == DialogResult.Yes)
                {
                    seleccionado = (Pokemon)dgvPokemons.CurrentRow.DataBoundItem;

                    if (logico)
                    {
                        negocio.eliminarLogico(seleccionado.Id);
                    }
                    else
                    {
                        negocio.eliminar(seleccionado.Id);
                    }

                    cargar();       // Este método es para actualizar la grilla una vez que eliminé un registro de la DB
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private bool validarFiltro()
        {
            // Quiero validar que los campos estén cargados: 
            
            if(cboCampo.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el campo.");
                return true;    // si el campo está vacío return true
            }

            if (cboCriterio.SelectedIndex < 0)
            {
                MessageBox.Show("Por favor, seleccione el criterio.");
                return true;    // si el campo está vacío return true
            }

            if (cboCampo.SelectedItem.ToString() == "Número")
            {
                if (string.IsNullOrEmpty(txtFiltroAvanzado.Text))       // si la caja de texto está vacía: 
                {
                    MessageBox.Show("Debe ingresar un número.");
                    return true;
                }

                if (!(soloNumeros(txtFiltroAvanzado.Text)))
                {
                    MessageBox.Show("Ingrese solo números por favor.");
                    return true;
                }
                return false;
            }

            return false;   // si está todo bien return false 
        }

        private bool soloNumeros(string cadena)      // este método es para el campo "Números"
        {
            // Recorrer la cadena y devolver true (si hay solo numeros) o false (si no hay solo numeros)

            foreach (char caracter in cadena)
            {
                if (!(char.IsNumber(caracter)))
                {
                    return false;
                }
            }
            return true;
        }

        private void btnFiltro_Click(object sender, EventArgs e)
        {

            PokemonNegocio negocio = new PokemonNegocio();      // voy a crear una instancia de esta clase porque necesito usar un método de la misma (listar)
                                                                // el PokemonNegocio me va a devolver una lista

            if (validarFiltro())
            {
                return;     // este return es para cortar la ejecución del evento
            }

            // Capturar los 3 campos (numero, nombre y descripcion)
            try
            {
                string campo = cboCampo.SelectedItem.ToString();
                string criterio = cboCriterio.SelectedItem.ToString();
                string filtro = txtFiltroAvanzado.Text;

                dgvPokemons.DataSource = negocio.filtrar(campo, criterio, filtro);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


          
            
        }

        private void txtFiltro_TextChanged(object sender, EventArgs e)
        {
            List<Pokemon> listaFiltrada;        // en esta lista van a aparecer los objetos que busque el usuario
                                                // funciona como una sublista de la lista de pokemons 

            string filtro = txtFiltro.Text;

            if (filtro.Length >= 2)       // si el usuario escribió dentro de la caja de texto para buscar un pokemon
            {
                // ¿Cómo llenar la lista 'listaFiltrada'?
                listaFiltrada = listaPokemon.FindAll(x => x.Nombre.ToUpper().Contains(filtro.ToUpper()) || x.Tipo.Descripcion.ToUpper().Contains(filtro.ToUpper()));      // Lambda expression

            }
            else
            {
                listaFiltrada = listaPokemon;
            }

            dgvPokemons.DataSource = null;      // primero limpiar el DataSource para después cargarle los datos de la listaFiltrada
            dgvPokemons.DataSource = listaFiltrada;
            ocultarColumna();
        }

        private void cboCampo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string opcion = cboCampo.SelectedItem.ToString();       // me va a guardar el elemento seleccionado

            if (opcion == "Número")
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Mayor a");
                cboCriterio.Items.Add("Menor a");
                cboCriterio.Items.Add("Igual a");
            } 
            else
            {
                cboCriterio.Items.Clear();
                cboCriterio.Items.Add("Comienza con");
                cboCriterio.Items.Add("Termina con");
                cboCriterio.Items.Add("Contiene");
            }
        }
    }
}
