using ApiDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace ApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosDemoController : ControllerBase
    {
		//Esta es la APIKEY para consumir los endpoints de la API
		public static string SEGURIDADPASS = "1234";
		//Aquí se asigna la cadena de conexión de nuestra base de datos, 
		//depende de la direccion de nuestro servidor, usuario, nombre de la DB
		public static string CADCNX = "AQUÍ VA LA CADENA DE CONEXION";

		//Método que recibe la cadena de conexion, y el script de la consulta
		//retorna la data obtenida de acuerdo a la consulta
		//en un formato de DataTable
		public static DataTable GetData(string Conn, string QRY1)
		{
			try
			{
				DataTable dt = new DataTable();
				SqlConnection conn = new SqlConnection(Conn);
				conn.Open();
				SqlCommand cmd = new SqlCommand(QRY1, conn);
				dt.Load(cmd.ExecuteReader());
				cmd.Dispose();
				conn.Close();
				conn.Dispose();
				return dt;
			}
			catch (Exception ex)
			{
				return null;
			}
		}

		//Método que recibe una cadena, un caracter de separacion y la posicion
		//Crea una lista de cadenas separadas por el caracter de separacion,
		//y retorna la cadena deseada de la lista en la posicion indicada.
		public static string GetStringCol(string cadena, string simbolo_separa, int posicion)
		{
			// busca el valor de una columna en una cadena separada por x caracter
			string[] lista = cadena.Split(new char[] { Convert.ToChar(simbolo_separa) });
			if (posicion < lista.Count())
				return lista[posicion].Trim();

			return "";
		}

		//El modo de consumir este endpoint es de la siguiente manera:
		//    http://[Dominio]/[NombreController]/[NombreMetodo]
		//Ej. https://localhost:7183/UsuariosDemo/GetUsuariosDemo
		[HttpPost("GetUsuariosDemo")]
		public List<UsuariosDemo> GetUsuariosDemo()
		{
			//Se consume el metodo GetStringCol, se envia por parámetros
			//parámetro 1: Una cadena, que es el titulo de la cabecera,
			//parámetro 2: El caracter de separacion
			//parámetro 3: El índice de la lista para retornar la cadena correspondiente						
			var Clave_ = GetStringCol(HttpContext.Request.Headers["Cabecera"], "|", 0);//el índice 0 indica la APIKEY recibida
			var Parametro_Where_Completo = GetStringCol(HttpContext.Request.Headers["Cabecera"], "|", 1); //el índice 1 indica la cláusula where
			var Parametro_Order_By_Completo = GetStringCol(HttpContext.Request.Headers["Cabecera"], "|", 2);//el índice 2 indica la cláusula order by
			//Si la clave recibida de la cabecera NO es igual a nuestra APIKEY, 
			//retorna una lista vacía de tipo UsuariosDemo
			if (!Clave_.Equals(SEGURIDADPASS)) return new List<UsuariosDemo>();
			//Se crea una lista de tipo UsuariosDemo (nuestro MODELO) 
			var lista_UsuariosDemo = new List<UsuariosDemo>();
			try
			{
				var DT = new DataTable();
				//Se consume el método GetData, que recibe:
				//la cadena de conexxion, el script/query, la cláusula where, la cláusula order by
				//guardamos la data que retorna de tipo DataTable
				DT = GetData(CADCNX, "SELECT * FROM UsuariosDemo " + Parametro_Where_Completo + " " + Parametro_Order_By_Completo);
				//Si existe data entra a la condicion
				if (DT.Rows.Count != 0)
				{
					//Se itera de acuerdo al total de registros
					//y se van creando los objetos de tipo UsuariosDemo
					//Y añadiendose a la lista
					for (int i = 0; i < DT.Rows.Count; i++)
					{
						var Element_UsuariosDemo = new UsuariosDemo
						{
							id = (decimal)DT.Rows[i]["ID"],
							nombre = (string)DT.Rows[i]["NOMBRE"],
							apellido = (string)DT.Rows[i]["APELLIDO"],
							edad = Convert.ToInt16(DT.Rows[i]["EDAD"]),
							sexo = (string)DT.Rows[i]["SEXO"],
						};
						lista_UsuariosDemo.Add(Element_UsuariosDemo);
					}
				}
				//Se retorna la lista de objetos de tipo UsuariosDemo
				return lista_UsuariosDemo;
			}
			//En caso de que el bloque  try no realice su funcion,
			//se atrapa el error y se retorna una lista vacia de tipo UsuariosDemo
			catch (Exception Ex)
			{
				return new List<UsuariosDemo>();
			}
		}

		//El modo de consumir este endpoint es de la siguiente manera:
		//    http://[Dominio]/[NombreController]/[NombreMetodo]/[id_Usuario]
		//Ej. https://localhost:7183/UsuariosDemo/GetUsuariosDemo/1
		[HttpPost("GetUsuariosDemo/{id}")]
		public IActionResult GetUsuariosDemo(int id)
		{
			UsuariosDemo usuario = new UsuariosDemo();

			var Clave_ = GetStringCol(HttpContext.Request.Headers["Cabecera"], "|", 0);
			var Parametro_Where_Completo = GetStringCol(HttpContext.Request.Headers["Cabecera"], "|", 1);
			var Parametro_Order_By_Completo = GetStringCol(HttpContext.Request.Headers["Cabecera"], "|", 2);
			if (!Clave_.Equals(SEGURIDADPASS)) return Unauthorized();
			try
			{
				var DT = new DataTable();
				DT = GetData(CADCNX, "SELECT * FROM UsuariosDemo where id="+id);
				if (DT.Rows.Count != 0)
				{
						usuario = new UsuariosDemo
						{
							id = (decimal)DT.Rows[0]["ID"],
							nombre = (string)DT.Rows[0]["NOMBRE"],
							apellido = (string)DT.Rows[0]["APELLIDO"],
							edad = Convert.ToInt16(DT.Rows[0]["EDAD"]),
							sexo = (string)DT.Rows[0]["SEXO"],
						};
					return Ok(usuario);
				}
                else
                {
					return NotFound("el id "+id+" no existe");
				}
				
			}
			catch (Exception Ex)
			{

				return NotFound(usuario);
			}
		}

		//El modo de consumir este endpoint es de la siguiente manera:
		//    http://[Dominio]/[NombreController]/[NombreMetodo]
		//Ej. https://localhost:7183/UsuariosDemo/UpUsuariosDemo
		//Este método recibe como parámetro un objeto de tipo UsuariosDemo
		[HttpPost("UpUsuariosDemo")]
		public string UpUsuariosDemo(UsuariosDemo UsuariosDemo_)
		{
			//Si el objeto que se recibió NO coincide con el MODELO que nosotros tenemos,
			//se retorna una cadena, con un mensaje que indica el problema
			if (!ModelState.IsValid) return "BadRequest(ModelState)";
			//Se consume el Método GetStringCol para obtener la APIKEY
			var Clave = GetStringCol(HttpContext.Request.Headers["Cabecera"], "|", 0);
			//Si la APIKEY recibida NO coincide con nuestra APIKEY asignada de manera estática,
			// retornará una cadena con la leyenda "Acceso Denegado"
			if (!Clave.Equals(SEGURIDADPASS)) return "Acceso Denegado";
			//El bloque try intentará ejecutar el código correspondiente,
			//Para inyectar el script a la DB con los parámetros que se solicitan
			//Al finalizar de manera correcta, retornará una cadena que indica que,
			//el registro se guardó con éxito
			try
			{
				var CNX = new SqlConnection();
				CNX.ConnectionString = CADCNX;
				CNX.Open();
				var Qry = "INSERT INTO UsuariosDemo(nombre,apellido,edad,sexo) VALUES('" + UsuariosDemo_.nombre + "','" + UsuariosDemo_.apellido + "'," + UsuariosDemo_.edad + ",'" + UsuariosDemo_.sexo + "')";
				var comando = new SqlCommand(Qry, CNX);
				comando.ExecuteNonQuery();
				CNX.Close();
				CNX.Dispose();
				CNX = null;
				return "Registro Guardado Exitosamente";
			}
			catch (Exception Ex)
			{
				return Ex.Message;
			}
		}

		//El modo de consumir este endpoint es de la siguiente manera:
		//    http://[Dominio]/[NombreController]/[NombreMetodo]
		//Ej. https://localhost:7183/UsuariosDemo/Update
		[HttpPost("Update")]
		public string UpdateTabla()
		{
			var Clave = GetStringCol(HttpContext.Request.Headers["Cabecera"], "|", 0);//Retorna la APIKEY
			var Query = GetStringCol(HttpContext.Request.Headers["Cabecera"], "|", 1);//Retorna el script/Query
			if (!Clave.Equals(SEGURIDADPASS)) return "Acceso Denegado";
			try
			{
				var CNX = new SqlConnection();
				CNX.ConnectionString = CADCNX;
				CNX.Open();
				string Qry = Query;
				var comando = new SqlCommand(Qry, CNX);
				comando.ExecuteNonQuery();
				CNX.Close();
				CNX.Dispose();
				CNX = null;
				return "El registro ha sido actualizado exitosamente";
			}
			catch (Exception Ex)
			{
				return Ex.Message;
			}
		}

	}
}
