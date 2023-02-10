using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Text;

namespace CotizacionApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Catalogos : ControllerBase
    {

        private string connectionString = @"Data Source=LAPTOP-BPKAPJHG\SQLEXPRESS;Database = catalogo_autos;Integrated Security=True";

        // Método para obtener todas las marcas
        [Route("/marcas")]
        [HttpGet]
        public IEnumerable<string> GetMarcas()
        {
            List<string> marcas = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT DISTINCT marca FROM catalogoE", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            marcas.Add(reader["marca"].ToString());
                        }
                    }
                }
            }

            return marcas;
        }

        // Método para obtener todas las submarcas de una marca
        [Route("/submarcas/{marca}")]
        [HttpGet]
        public IEnumerable<string> GetSubMarcas(string marca)
        {
            List<string> submarcas = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT DISTINCT Submarca FROM catalogoE WHERE Marca = @marca", connection))
                {
                    command.Parameters.AddWithValue("@marca", marca);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            submarcas.Add(reader["Submarca"].ToString());
                        }
                    }
                }
            }

            return submarcas;
        }

        // Método para obtener todos los modelos de una submarca
        [Route("/modelos/{submarca}")]
        [HttpGet]
        public IEnumerable<string> GetModelos(string submarca)
        {
            List<string> modelos = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT DISTINCT Modelo FROM catalogoE WHERE Submarca = @submarca", connection))
                {
                    command.Parameters.AddWithValue("@submarca", submarca);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            modelos.Add(reader["Modelo"].ToString());
                        }
                    }
                }
            }

            return modelos;
        }

        public class DescripcionModel
        {
            public string Descripcion { get; set; }
            public string DescripcionId { get; set; }
        }

        [Route("/modelos/{submarca}/{modelo}")]
        [HttpGet]
        public IEnumerable<DescripcionModel> GetDescripcion(string submarca, string modelo)
        {
            List<DescripcionModel> descripciones = new List<DescripcionModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT descripcion,DescripcionId FROM catalogoE WHERE Submarca = @submarca and Modelo = @modelo", connection))
                {
                    command.Parameters.AddWithValue("@submarca", submarca);
                    command.Parameters.AddWithValue("@modelo", modelo);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            descripciones.Add(new DescripcionModel
                            {
                                Descripcion = reader["descripcion"].ToString(),
                                DescripcionId = reader["DescripcionId"].ToString()
                            });
                        }
                    }
                }
            }

            return descripciones;
        }

        private readonly HttpClient client;

        public Catalogos(HttpClient httpClient)
        {
            client = httpClient;
        }


        [HttpGet("/domicilio/{codigoPostal}")]
        public async Task<IActionResult> GetSepomex(string codigoPostal)
        {

            var response = await client.GetAsync($"https://api-test.aarco.com.mx/api-examen/api/examen/sepomex/{codigoPostal}");
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var deserializedResult = JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                var catalogoJsonString = deserializedResult["CatalogoJsonString"].ToString();
                var cleanResult = JsonConvert.DeserializeObject<List<Model.Direccion>>(catalogoJsonString);

                return Ok(cleanResult);
            }

            return BadRequest(result);
        }

        [HttpPost("/peticion")]
        public async Task<IActionResult> CreatePeticionn([FromBody] Model.Peticion peticion)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api-test.aarco.com.mx/api-examen/api/examen/crear-peticion");
            request.Content = new StringContent(JsonConvert.SerializeObject(peticion), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("/peticionFinal/{peticionLlave}")]
        public async Task<IActionResult> GetPeticion(string peticionLlave)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync($"https://api-test.aarco.com.mx/api-examen/api/examen/peticion/{peticionLlave}");
                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var respuesta = JsonConvert.DeserializeObject<Model.Respuesta>(result);
                    return Ok(respuesta);
                }

                return BadRequest(result);
            }
        }




    }
}