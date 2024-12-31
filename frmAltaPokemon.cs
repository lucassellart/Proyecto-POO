using dominio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using negocio;
using System.IO;
using System.Configuration;
using static System.Net.Mime.MediaTypeNames;

namespace winform_app
{
    public partial class frmAltaPokemon : Form
    {
        private Pokemon pokemon = null;     // atributo privado pokemon

        private OpenFileDialog archivo = null;      // atributo privado para agg imagen desde carpeta computadora

        public frmAltaPokemon()
        {
            InitializeComponent();
        }

        public frmAltaPokemon(Pokemon pokemon)
        {
            InitializeComponent();

            this.pokemon = pokemon;

            Text = "Modificar Pokemon";
        }


        private void btnAceptar_Click(object sender, EventArgs e)
        {

            //Pokemon poke = new Pokemon();

            PokemonNegocio negocio = new PokemonNegocio();

            try
            {

                if (pokemon == null)
                {
                    pokemon = new Pokemon();
                }

                pokemon.Numero = int.Parse(txtNumero.Text);
                pokemon.Nombre = txtNombre.Text;
                pokemon.Descripcion = txtDescripcion.Text;

                pokemon.UrlImagen = txtUrlImagen.Text;     // capturar el valor de la imagen al usar URL de internet

                pokemon.Tipo = (Elemento)cbxTipo.SelectedItem;
                pokemon.Debilidad = (Elemento)cbxDebilidad.SelectedItem;


                if (pokemon.Id != 0)
                {
                    negocio.modificar(pokemon);
                    MessageBox.Show("Pokemon modificado correctamente");

                } else
                {
                    negocio.agregar(pokemon);
                    MessageBox.Show("Pokemon cargado exitosamente");

                }

                // Guardo la imagen si la levantó localmente: 

                if (archivo != null && !(txtUrlImagen.Text.ToUpper().Contains("HTTP")));       // asegurarme de que el usuario quiere guardar un archivo local
                {
                    File.Copy(archivo.FileName, ConfigurationManager.AppSettings["images-folder"] + archivo.SafeFileName);        // ir a App.config
                                                                                                                                  // para usar "Configuration Manager" hay que agregar: using System.Configuration;
                }

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmAltaPokemon_Load(object sender, EventArgs e)
        {

            ElementoNegocio elementoNegocio = new ElementoNegocio();

            try
            {
                cbxTipo.DataSource = elementoNegocio.listar();

                cbxTipo.ValueMember = "Id";
                cbxTipo.DisplayMember = "Descripcion";

                cbxDebilidad.DataSource = elementoNegocio.listar();

                cbxDebilidad.ValueMember = "Id";
                cbxDebilidad.DisplayMember = "Descripcion";

                if (pokemon != null)        // significa que tengo un pokemon para modificar
                {                           // tengo que precargar los datos en el modificar

                    txtNumero.Text = pokemon.Numero.ToString();
                    txtNombre.Text = pokemon.Nombre;
                    txtDescripcion.Text = pokemon.Descripcion;
                    txtUrlImagen.Text = pokemon.UrlImagen;
                    cargarImagen(pokemon.UrlImagen);

                    cbxTipo.SelectedValue = pokemon.Tipo.Id;
                    cbxDebilidad.SelectedValue = pokemon.Debilidad.Id;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void cargarImagen(string imagen)
        {
            try
            {
                pbxPokemon.Load(imagen);
            }
            catch (Exception ex)
            {
                pbxPokemon.Load("https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcT9cSGzVkaZvJD5722MU5A-JJt_T5JMZzotcw&s");
            }


        }

        private void txtUrlImagen_Leave(object sender, EventArgs e)
        {

            cargarImagen(txtUrlImagen.Text);
        }

        private void btnAgregarImagen_Click(object sender, EventArgs e)
        {
            // Para poder levantar una imagen desde nuestra computadora: 

            archivo = new OpenFileDialog();      // abre una ventana de diálogo y me permite elegir un archivo
            archivo.Filter = "jpg|*.jpg|png|*.png";       // le digo que me traiga todos los archivos jpg 
            
            if (archivo.ShowDialog() == DialogResult.OK)
            {
                txtUrlImagen.Text = archivo.FileName;       // esto me va a guardar la ruta completa del archivo que estoy seleccionando

                cargarImagen(archivo.FileName);     // llamo a este método para ver el archivo que estoy capturando

                // Guardar el archivo: 

                //

            }
        }
    }
}
