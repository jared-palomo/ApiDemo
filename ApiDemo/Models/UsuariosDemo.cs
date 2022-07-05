namespace ApiDemo.Models
{
    //UsuariosDemo es una Clase,
    //que representa el modelo de la tabla,
    //con el mismo nombre en la DB
    public class UsuariosDemo
    {
        public decimal id { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public int edad { get; set; }
        public string sexo { get; set; }
    }
}
