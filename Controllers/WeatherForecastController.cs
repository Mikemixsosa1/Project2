using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Project2.Models;


namespace Project2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration _configuration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, EscuelaContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("simple")]
        public ActionResult SimpleLogin()
        {
            return Ok("El método PostLogin es accesible.");
        }


        private readonly EscuelaContext _context;



        [HttpPost("login")]
        public async Task<ActionResult> PostLogin(UserLoginDto userLogin)
        {

            var user = await _context.Usuarios
                                     .Where(u => u.Username == userLogin.Username && u.Password == userLogin.Password)
                                     .FirstOrDefaultAsync();

            if (user == null)
            {
                return Unauthorized("Credenciales incorrectas.");
            }

            // Devuelve el tipo de usuario
            return Ok(new { userType = user.TipoUsuario });
        }


        [HttpGet("findstudent/{curp}")]
        public async Task<ActionResult<Estudiante>> GetEstudianteByCurp(string curp)
        {
            var estudiante = await _context.Estudiantes
                                           .Include(e => e.NivelEducativo)
                                           .Include(e => e.Domicilio)
                                           .FirstOrDefaultAsync(e => e.CURP == curp);

            if (estudiante == null)
            {
                return NotFound("Estudiante no encontrado.");
            }

            return estudiante;
        }

        [HttpGet("GetPaises")]
        public async Task<ActionResult<IEnumerable<Pais>>> GetPaises()
        {
            return await _context.Paises.ToListAsync();
        }

        [HttpGet("GetEstadoByPais/{paisId}")]
        public async Task<ActionResult<IEnumerable<Estado>>> GetEstados(int paisId)
        {
            return await _context.Estados.Where(e => e.IdPais == paisId).ToListAsync();
        }

        [HttpGet("GetMunicipioByEstado/{estadoId}")]
        public async Task<ActionResult<IEnumerable<Municipio>>> GetMunicipios(int estadoId)
        {
            return await _context.Municipios.Where(m => m.IdEstado == estadoId).ToListAsync();
        }

        [HttpGet("GetNivelesEducativos")]
        public async Task<ActionResult<IEnumerable<NivelEducativo>>> GetNivelesEducativos()
        {
            return await _context.NivelesEducativos.ToListAsync();
        }

        [HttpGet("GetEstadoById/{id}")]
        public async Task<ActionResult<Estado>> GetEstadoById(int id)
        {
            var estado = await _context.Estados.FindAsync(id);

            if (estado == null)
            {
                return NotFound("Estado no encontrado.");
            }

            return estado;
        }

        [HttpGet("GetMunicipioById/{id}")]
        public async Task<ActionResult<Municipio>> GetMunicipioById(int id)
        {
            var municipio = await _context.Municipios.FindAsync(id);

            if (municipio == null)
            {
                return NotFound("Municipio no encontrado.");
            }

            return municipio;
        }


        [HttpPost("addstudent")]
        public IActionResult AddEstudiante(_Estudiante estudiante)
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


        [HttpPut("updatestudent/{curp}")]
        public IActionResult UpdateEstudiante(string curp, _Estudiante estudiante)
        {
            if (estudiante == null || curp != estudiante.CURP)
            {
                return BadRequest("Datos del estudiante inválidos.");
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    // Iniciar una transacción
                    using (MySqlTransaction transaction = conn.BeginTransaction())
                    {
                        // Actualizar domicilio
                        if (estudiante.Domicilio != null)
                        {
                            string updateDomicilioSql = "UPDATE Domicilios SET calle = @Calle, colonia = @Colonia, idmunicipio = @IdMunicipio, idestado = @IdEstado, idpais = @IdPais WHERE iddomicilio = (SELECT iddomicilio FROM Estudiantes WHERE CURP = @CURP);";
                            MySqlCommand updateDomicilioCmd = new MySqlCommand(updateDomicilioSql, conn, transaction);
                            updateDomicilioCmd.Parameters.AddWithValue("@Calle", estudiante.Domicilio.Calle);
                            updateDomicilioCmd.Parameters.AddWithValue("@Colonia", estudiante.Domicilio.Colonia);
                            updateDomicilioCmd.Parameters.AddWithValue("@IdMunicipio", estudiante.Domicilio.IdMunicipio);
                            updateDomicilioCmd.Parameters.AddWithValue("@IdEstado", estudiante.Domicilio.IdEstado);
                            updateDomicilioCmd.Parameters.AddWithValue("@IdPais", estudiante.Domicilio.IdPais);
                            updateDomicilioCmd.Parameters.AddWithValue("@CURP", curp);
                            updateDomicilioCmd.ExecuteNonQuery();
                        }

                        // Actualizar estudiante
                        string updateEstudianteSql = "UPDATE Estudiantes SET nombre = @Nombre, paterno = @Paterno, materno = @Materno, idniveleducativo = @IdNivelEducativo, telefonoEstudiante = @TelefonoEstudiante WHERE CURP = @CURP;";
                        MySqlCommand updateEstudianteCmd = new MySqlCommand(updateEstudianteSql, conn, transaction);
                        updateEstudianteCmd.Parameters.AddWithValue("@Nombre", estudiante.Nombre);
                        updateEstudianteCmd.Parameters.AddWithValue("@Paterno", estudiante.Paterno);
                        updateEstudianteCmd.Parameters.AddWithValue("@Materno", estudiante.Materno);
                        updateEstudianteCmd.Parameters.AddWithValue("@IdNivelEducativo", estudiante.IdNivelEducativo);
                        updateEstudianteCmd.Parameters.AddWithValue("@TelefonoEstudiante", estudiante.TelefonoEstudiante);
                        updateEstudianteCmd.Parameters.AddWithValue("@CURP", curp);
                        updateEstudianteCmd.ExecuteNonQuery();

                        // Commit de la transacción
                        transaction.Commit();
                    }

                    conn.Close();
                }

                return Ok("Estudiante actualizado con éxito.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al actualizar estudiante: {ex.Message}");
            }
        }

        [HttpDelete("deletestudent/{curp}")]
        public IActionResult DeleteEstudiante(string curp)
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
                        // Eliminar estudiante
                        string deleteEstudianteSql = "DELETE FROM Estudiantes WHERE CURP = @CURP;";
                        MySqlCommand deleteEstudianteCmd = new MySqlCommand(deleteEstudianteSql, conn, transaction);
                        deleteEstudianteCmd.Parameters.AddWithValue("@CURP", curp);
                        int rowsAffected = deleteEstudianteCmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            // Si no se afectaron filas, el estudiante no existe
                            return NotFound("Estudiante no encontrado.");
                        }

                        string deleteDomicilioSql = "DELETE FROM Domicilios WHERE iddomicilio = (SELECT iddomicilio FROM Estudiantes WHERE CURP = @CURP);";
                        MySqlCommand deleteDomicilioCmd = new MySqlCommand(deleteDomicilioSql, conn, transaction);
                        deleteDomicilioCmd.Parameters.AddWithValue("@CURP", curp);
                        deleteDomicilioCmd.ExecuteNonQuery();

                        transaction.Commit();
                    }

                    conn.Close();
                }

                return Ok("Estudiante eliminado con éxito.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al eliminar estudiante: {ex.Message}");
            }
        }

        [HttpGet("GetAsuntos")]
        public async Task<ActionResult<IEnumerable<Asunto>>> GetAsuntos()
        {
            return await _context.Asuntos.ToListAsync();
        }

        [HttpGet("GetMunicipiosPorCURP/{curp}")]
        public async Task<ActionResult<IEnumerable<Municipio2>>> GetMunicipiosPorCURP(string curp)
        {
            int idEstado = ObtenerIdEstadoPorCURP(curp);
            if (idEstado == 0)
            {
                return NotFound("Estado no encontrado para el CURP proporcionado.");
            }

            return await GetMunicipios2(idEstado);
        }

        private int ObtenerIdEstadoPorCURP(string curp)
        {
            int idEstado = 0;
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = @"SELECT D.idestado FROM Estudiantes E 
                               JOIN Domicilios D ON E.iddomicilio = D.iddomicilio 
                               WHERE E.CURP = @CURP";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@CURP", curp);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idEstado = reader.GetInt32(0);
                        }
                    }
                }
            }

            return idEstado;
        }


        private async Task<ActionResult<IEnumerable<Municipio2>>> GetMunicipios2(int idEstado)
        {
            var municipios = new List<Municipio2>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sql = "SELECT idmunicipio, nombremunicipio FROM Municipios WHERE idestado = @idEstado";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idEstado", idEstado);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var municipio = new Municipio2
                            {
                                IdMunicipio = reader.GetInt32(0),
                                NombreMunicipio = reader.GetString(1)
                            };
                            municipios.Add(municipio);
                        }
                    }
                }
            }

            return municipios;
        }

        [HttpPost("crearTicket2")]
        public ActionResult CrearTicket2([FromBody] Ticket2 ticket)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    ticket.FechaRegistroTramite = DateTime.Now;
                    ticket.Estatus = "En Progreso";
                    ticket.FechaDeTramite = ObtenerFechaDeTramite(connection, ticket.IdMunicipio);
                    ticket.NumeroTurno = ObtenerNumeroTurno(connection, ticket.IdMunicipio, ticket.FechaDeTramite.Value);
                    ticket.Estatus = "En Progreso"; // Establecer Estatus aquí
                    string insertSql = "INSERT INTO Tickets (CURP, idasunto, estatus, numeroturno, FechaRegistroTramite, FechaDeTramite, idmunicipio) VALUES (@CURP, @IdAsunto, @Estatus, @NumeroTurno, @FechaRegistroTramite, @FechaDeTramite, @IdMunicipio);";
                    MySqlCommand command = new MySqlCommand(insertSql, connection);
                    command.Parameters.AddWithValue("@CURP", ticket.CURP);
                    command.Parameters.AddWithValue("@IdAsunto", ticket.IdAsunto);
                    command.Parameters.AddWithValue("@Estatus", ticket.Estatus);
                    command.Parameters.AddWithValue("@NumeroTurno", ticket.NumeroTurno);
                    command.Parameters.AddWithValue("@FechaRegistroTramite", ticket.FechaRegistroTramite);
                    command.Parameters.AddWithValue("@FechaDeTramite", ticket.FechaDeTramite);
                    command.Parameters.AddWithValue("@IdMunicipio", ticket.IdMunicipio);
                    command.ExecuteNonQuery();

                    long folio = command.LastInsertedId;


                    string asuntoSql = "SELECT descripcion FROM Asuntos WHERE idasunto = @IdAsunto;";
                    MySqlCommand asuntoCommand = new MySqlCommand(asuntoSql, connection);
                    asuntoCommand.Parameters.AddWithValue("@IdAsunto", ticket.IdAsunto);
                    string descripcionAsunto = (string)asuntoCommand.ExecuteScalar();

                    // Crear un objeto para enviar como respuesta
                    var ticketResponse = new
                    {
                        Folio = folio,
                        FechaDeTramite = ticket.FechaDeTramite,
                        DescripcionAsunto = descripcionAsunto, 
                        NumeroTurno = ticket.NumeroTurno 
                    };

                    return Ok(ticketResponse);

                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        private DateTime ObtenerFechaDeTramite(MySqlConnection connection, int idMunicipio)
        {
            DateTime fechaDeTramite = DateTime.Now.Date; // Comenzar con la fecha actual
            while (true)
            {
                // Saltar fines de semana
                if (fechaDeTramite.DayOfWeek == DayOfWeek.Saturday)
                {
                    fechaDeTramite = fechaDeTramite.AddDays(2); // Avanzar al lunes
                    continue;
                }
                else if (fechaDeTramite.DayOfWeek == DayOfWeek.Sunday)
                {
                    fechaDeTramite = fechaDeTramite.AddDays(1); // Avanzar al lunes
                    continue;
                }

                // Consulta para contar los tickets asignados para esta fecha y municipio
                string sql = "SELECT COUNT(*) FROM Tickets WHERE idmunicipio = @IdMunicipio AND DATE(FechaDeTramite) = @FechaDeTramite;";
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@IdMunicipio", idMunicipio);
                    command.Parameters.AddWithValue("@FechaDeTramite", fechaDeTramite);

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    if (count < 10)
                    {
                        break; // Menos de 10 tickets, usar esta fecha
                    }
                }

                fechaDeTramite = fechaDeTramite.AddDays(1); // Avanzar al siguiente día si el límite de 10 se ha alcanzado
            }
            return fechaDeTramite;
        }




        private int ObtenerNumeroTurno(MySqlConnection connection, int idMunicipio, DateTime fechaDeTramite)
        {
            string sql = "SELECT COALESCE(MAX(numeroturno), 0) FROM Tickets WHERE idmunicipio = @IdMunicipio AND DATE(FechaDeTramite) = @FechaDeTramite;";
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@IdMunicipio", idMunicipio);
                command.Parameters.AddWithValue("@FechaDeTramite", fechaDeTramite.Date); // Asegúrate de que solo se compare la fecha

                int maxTurno = Convert.ToInt32(command.ExecuteScalar());
                return maxTurno + 1;
            }
        }

        [HttpGet("buscarTicket")]
        public ActionResult BuscarTicket(int folio, string curp)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string selectSql = "SELECT * FROM Tickets WHERE Folio = @Folio AND CURP = @CURP;";
                    MySqlCommand command = new MySqlCommand(selectSql, connection);
                    command.Parameters.AddWithValue("@Folio", folio);
                    command.Parameters.AddWithValue("@CURP", curp);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            _Ticket ticket = new _Ticket()
                            {
                                Folio = Convert.ToInt32(reader["Folio"]),
                                CURP = reader["CURP"].ToString(),
                                IdAsunto = Convert.ToInt32(reader["idasunto"]),
                                Estatus = reader["estatus"].ToString(),
                                NumeroTurno = Convert.ToInt32(reader["numeroturno"]),
                                FechaRegistroTramite = Convert.ToDateTime(reader["FechaRegistroTramite"]),
                                FechaDeTramite = Convert.ToDateTime(reader["FechaDeTramite"]),
                                IdMunicipio = Convert.ToInt32(reader["idmunicipio"])
                            };

                            return Ok(ticket);
                        }
                        else
                        {
                            return NotFound("Ticket no encontrado.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost("actualizarTicket")]
        public ActionResult ActualizarTicket([FromBody] Ticket2 ticket)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Reasignamos el turno y la fecha de trámite
                    ticket.FechaDeTramite = ObtenerFechaDeTramite(connection, ticket.IdMunicipio);
                    ticket.NumeroTurno = ObtenerNumeroTurno(connection, ticket.IdMunicipio, ticket.FechaDeTramite.Value);

                    // Actualizar el ticket existente (excluyendo Folio)
                    string updateSql = "UPDATE Tickets SET idasunto = @IdAsunto, estatus = @Estatus, numeroturno = @NumeroTurno, FechaRegistroTramite = @FechaRegistroTramite, FechaDeTramite = @FechaDeTramite, idmunicipio = @IdMunicipio WHERE Folio = @Folio;";
                    MySqlCommand command = new MySqlCommand(updateSql, connection);
                    command.Parameters.AddWithValue("@Folio", ticket.Folio); // Usado solo en la cláusula WHERE
                    command.Parameters.AddWithValue("@IdAsunto", ticket.IdAsunto);
                    command.Parameters.AddWithValue("@Estatus", ticket.Estatus);
                    command.Parameters.AddWithValue("@NumeroTurno", ticket.NumeroTurno);
                    command.Parameters.AddWithValue("@FechaRegistroTramite", ticket.FechaRegistroTramite);
                    command.Parameters.AddWithValue("@FechaDeTramite", ticket.FechaDeTramite);
                    command.Parameters.AddWithValue("@IdMunicipio", ticket.IdMunicipio);
                    command.ExecuteNonQuery();

                    return Ok(new { message = "Ticket actualizado con éxito" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }



    }



}