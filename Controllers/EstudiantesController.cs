using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Project2.Controllers
{
    [ApiController]
    [Route("Estudiantes")]
    public class EstudiantesController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public EstudiantesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("addstudent")]
        public IActionResult AddEstudiante(Estudiante estudiante)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Iniciar una transacción
                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        // Insertar domicilio
                        string insertDomicilioSql = "INSERT INTO Domicilios (calle, colonia, idmunicipio, idestado, idpais) VALUES (@Calle, @Colonia, @IdMunicipio, @IdEstado, @IdPais);";
                        MySqlCommand insertDomicilioCmd = new MySqlCommand(insertDomicilioSql, conn, transaction);
                        insertDomicilioCmd.Parameters.AddWithValue("@Calle", estudiante.Domicilio.Calle);
                        insertDomicilioCmd.Parameters.AddWithValue("@Colonia", estudiante.Domicilio.Colonia);
                        insertDomicilioCmd.Parameters.AddWithValue("@IdMunicipio", estudiante.Domicilio.IdMunicipio);
                        insertDomicilioCmd.Parameters.AddWithValue("@IdEstado", estudiante.Domicilio.IdEstado);
                        insertDomicilioCmd.Parameters.AddWithValue("@IdPais", estudiante.Domicilio.IdPais);
                        insertDomicilioCmd.ExecuteNonQuery();
                        long domicilioId = insertDomicilioCmd.LastInsertedId;

                        // Insertar estudiante
                        string insertEstudianteSql = "INSERT INTO Estudiantes (CURP, nombre, paterno, materno, idniveleducativo, iddomicilio, telefonoEstudiante) VALUES (@CURP, @Nombre, @Paterno, @Materno, @IdNivelEducativo, @IdDomicilio, @TelefonoEstudiante);";
                        MySqlCommand insertEstudianteCmd = new MySqlCommand(insertEstudianteSql, conn, transaction);
                        insertEstudianteCmd.Parameters.AddWithValue("@CURP", estudiante.CURP);
                        insertEstudianteCmd.Parameters.AddWithValue("@Nombre", estudiante.Nombre);
                        insertEstudianteCmd.Parameters.AddWithValue("@Paterno", estudiante.Paterno);
                        insertEstudianteCmd.Parameters.AddWithValue("@Materno", estudiante.Materno);
                        insertEstudianteCmd.Parameters.AddWithValue("@IdNivelEducativo", estudiante.IdNivelEducativo);
                        insertEstudianteCmd.Parameters.AddWithValue("@IdDomicilio", domicilioId);
                        insertEstudianteCmd.Parameters.AddWithValue("@TelefonoEstudiante", estudiante.TelefonoEstudiante);
                        insertEstudianteCmd.ExecuteNonQuery();

                        // Commit de la transacción
                        transaction.Commit();
                    }

                    conn.Close();
                }

                return Ok("Estudiante agregado con éxito.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al agregar estudiante: {ex.Message}");
            }
        }
    }
}
