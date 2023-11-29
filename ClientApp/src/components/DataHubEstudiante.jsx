import React, { useState, useEffect } from 'react';
import '../styles/DataHubEstudiante.css';
import { Input } from 'antd';
import { Select, Space } from 'antd';
import { Button, Flex } from 'antd';
import { useUser } from '../contexts/UserContext';
import { message } from 'antd';

const DataHubEstudiante = ({ curpProp }) => {
    const [paises, setPaises] = useState([]);
    const [estados, setEstados] = useState([]);
    const [municipios, setMunicipios] = useState([]);
    const [paisSeleccionado, setPaisSeleccionado] = useState(null);
    const [estadoSeleccionado, setEstadoSeleccionado] = useState(null);
    const [municipioSeleccionado, setMunicipioSeleccionado] = useState(null);
    const [nivelesEducativos, setNivelesEducativos] = useState([]);
    const { userType } = useUser();
    const [accionSeleccionada, setAccionSeleccionada] = useState(null);
    const [nivelEducativoSeleccionado, setNivelEducativoSeleccionado] = useState(null);

    const [curp, setCurp] = useState('');
    const [nombre, setNombre] = useState('');
    const [paterno, setPaterno] = useState('');
    const [materno, setMaterno] = useState('');
    const [telefono, setTelefono] = useState('');
    const [calle, setCalle] = useState('');
    const [colonia, setColonia] = useState('');
    const [curpEditable, setCurpEditable] = useState('');

    useEffect(() => {
        // Si se pasa una CURP como prop, úsala para inicializar el estado
        if (curpProp) {
            setCurpEditable(curpProp);
        }
    }, [curpProp]);

    const limpiarCampos = () => {
        setCurp('');
        setNombre('');
        setPaterno('');
        setMaterno('');
        setTelefono('');
        setCalle('');
        setColonia('');
        setPaisSeleccionado(null);
        setEstadoSeleccionado(null);
        setMunicipioSeleccionado(null);
        setNivelEducativoSeleccionado(null);
    };

    const validarCurp = (curp) => {
        const curpRegex = /^[A-Z]{4}[0-9]{6}[H,M][A-Z]{5}[A-Z0-9]{2}$/;
        return curpRegex.test(curp);
    };


    const onButtonClick = async () => {

        if (!validarCurp(curpProp)) {
            message.error('La CURP ingresada no es válida. Por favor, verifica la CURP e inténtalo de nuevo.');
            return;
        }

        if (accionSeleccionada === 'leer') {
            try {
                const respuesta = await fetch(`http://localhost:44429/weatherforecast/findstudent/${curpProp}`);
                if (respuesta.ok) {
                    const datos = await respuesta.json();
                    setNombre(datos.nombre);
                    setPaterno(datos.paterno);
                    setMaterno(datos.materno);
                    setTelefono(datos.telefonoEstudiante);
                    setCalle(datos.domicilio.calle);
                    setColonia(datos.domicilio.colonia);
                    setNivelEducativoSeleccionado(datos.idNivelEducativo);
                    // Establece los valores de los Select (país, estado, municipio, nivel educativo)
                    const paisEncontrado = paises.find(p => p.value === datos.domicilio.idPais);
                    setPaisSeleccionado(paisEncontrado ? paisEncontrado.value : null);

                    const respuestaEstados = await fetch(`http://localhost:44429/weatherforecast/GetEstadoByPais/${datos.domicilio.idPais}`);
                    if (respuestaEstados.ok) {
                        const datosEstados = await respuestaEstados.json();
                        setEstados(datosEstados.map(estado => ({ value: estado.idEstado, label: estado.nombreEstado })));

                        // Encuentra el estado correspondiente al domicilio del estudiante
                        const estadoEncontrado = datosEstados.find(e => e.idEstado === datos.domicilio.idEstado);

                        // Aquí, asegúrate de que estés pasando el idEstado correcto
                        if (estadoEncontrado) {
                            setEstadoSeleccionado(estadoEncontrado.idEstado);
                        } else {
                            setEstadoSeleccionado(null);
                        }
                        const respuestaMunicipios = await fetch(`http://localhost:44429/weatherforecast/GetMunicipioByEstado/${datos.domicilio.idEstado}`);
                        if (respuestaMunicipios.ok) {
                            const datosMunicipios = await respuestaMunicipios.json();
                            setMunicipios(datosMunicipios.map(municipio => ({ value: municipio.idMunicipio, label: municipio.nombreMunicipio })));

                            // Encuentra el municipio correspondiente al domicilio del estudiante
                            const municipioEncontrado = datosMunicipios.find(m => m.idMunicipio === datos.domicilio.idMunicipio);

                            if (municipioEncontrado) {
                                setMunicipioSeleccionado(municipioEncontrado.idMunicipio);
                            } else {
                                setMunicipioSeleccionado(null);
                            }
                        }
                    }
                } else {
                }
            } catch (error) {
                console.error('Error al buscar el estudiante:', error);
            }
        } else if (accionSeleccionada == 'crear') {
            try {
                const nuevoEstudiante = {
                    CURP: curpProp,
                    Nombre: nombre,
                    Paterno: paterno,
                    Materno: materno,
                    IdNivelEducativo: nivelEducativoSeleccionado,
                    TelefonoEstudiante: telefono,
                    Domicilio: {
                        Calle: calle,
                        Colonia: colonia,
                        IdPais: paisSeleccionado,
                        IdEstado: estadoSeleccionado,
                        IdMunicipio: municipioSeleccionado,
                    }
                };

                const respuesta = await fetch('http://localhost:44429/weatherforecast/addstudent', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(nuevoEstudiante),
                });

                if (respuesta.ok) {
                    message.success('Estudiante Registrado con exito');
                    limpiarCampos();

                } else {
                    message.error('Ocurrio un error');

                }
            } catch (error) {
                console.error('Error al crear el estudiante:', error);
                // Maneja errores de la solicitud aquí
            }
        } else if (accionSeleccionada === 'actualizar') {
            try {
                const estudianteActualizado = {
                    CURP: curpProp,
                    Nombre: nombre,
                    Paterno: paterno,
                    Materno: materno,
                    IdNivelEducativo: nivelEducativoSeleccionado,
                    TelefonoEstudiante: telefono,
                    Domicilio: {
                        Calle: calle,
                        Colonia: colonia,
                        IdPais: paisSeleccionado,
                        IdEstado: estadoSeleccionado,
                        IdMunicipio: municipioSeleccionado,
                    }
                };

                const respuesta = await fetch(`http://localhost:44429/weatherforecast/updatestudent/${curpProp}`, {
                    method: 'PUT',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(estudianteActualizado),
                });

                if (respuesta.ok) {
                    message.success('Estudiante actualizado con éxito');
                    limpiarCampos();
                } else {
                    message.error('Ocurrió un error al actualizar el estudiante');
                }
            } catch (error) {
                console.error('Error al actualizar el estudiante:', error);
                message.error('Error al actualizar el estudiante');
            }
        } else if (accionSeleccionada === 'eliminar') {
            try {
                const respuesta = await fetch(`http://localhost:44429/weatherforecast/deletestudent/${curpProp}`, {
                    method: 'DELETE'
                });

                if (respuesta.ok) {
                    message.success('Estudiante eliminado con éxito');
                    limpiarCampos();
                } else {
                    message.error('Ocurrió un error al eliminar el estudiante');
                }
            } catch (error) {
                console.error('Error al eliminar el estudiante:', error);
                message.error('Error al eliminar el estudiante');
            }
        }




    };

    const accionesCRUD = [
        { value: 'crear', label: 'Crear' },
        { value: 'leer', label: 'Buscar' },
        { value: 'actualizar', label: 'Actualizar' },
        { value: 'eliminar', label: 'Eliminar' }
    ];

    const opcionesAcciones = userType === 'administrador' ? accionesCRUD : accionesCRUD.filter(accion => accion.value === 'leer' || accion.value === 'crear');

    //Cargar Paises
    useEffect(() => {
        const cargarPaises = async () => {
            const respuesta = await fetch('http://localhost:44429/weatherforecast/GetPaises');
            const datos = await respuesta.json();
            setPaises(datos.map(pais => ({ value: pais.idPais, label: pais.nombrePais })));
        };
        cargarPaises();
    }, []);

    // Cargar estados cuando se seleccione un pais
    useEffect(() => {
        const cargarEstados = async () => {
            if (!paisSeleccionado) {
                setEstados([]);
                return;
            }
            const respuesta = await fetch(`http://localhost:44429/weatherforecast/GetEstadoByPais/${paisSeleccionado}`);
            const datos = await respuesta.json();
            setEstados(datos.map(estado => ({ value: estado.idEstado, label: estado.nombreEstado })));
        };
        cargarEstados();
    }, [paisSeleccionado]);



    // Cargar municipios cuando se seleccione un estado
    useEffect(() => {
        const cargarMunicipios = async () => {
            if (!estadoSeleccionado) {
                setMunicipios([]);
                return;
            }
            const respuesta = await fetch(`http://localhost:44429/weatherforecast/GetMunicipioByEstado/${estadoSeleccionado}`);
            const datos = await respuesta.json();
            setMunicipios(datos.map(municipio => ({ value: municipio.idMunicipio, label: municipio.nombreMunicipio })));
        };
        cargarMunicipios();
    }, [estadoSeleccionado]);

    //Cargar Niveles Educativos
    useEffect(() => {
        const cargarNivelesEducativos = async () => {
            const respuesta = await fetch('http://localhost:44429/weatherforecast/GetNivelesEducativos');
            const datos = await respuesta.json();
            setNivelesEducativos(datos.map(nivel => ({ value: nivel.idNivelEducativo, label: nivel.descripcion })));
        };
        cargarNivelesEducativos();
    }, []);

    const obtenerTextoBoton = () => {
        const accion = opcionesAcciones.find(op => op.value === accionSeleccionada);
        return accion ? accion.label : 'Ejecutar Acción';
    };


    return (
        <div className="grid-container">
            {/* Elementos de la columna izquierda */}
            <div className="grid-item" style={{ gridColumn: 1, gridRow: 1 }}>CURP:</div>
            <div className="grid-item" style={{ gridColumn: 1, gridRow: 2 }}>Nombre:</div>
            <div className="grid-item" style={{ gridColumn: 1, gridRow: 3 }}>Apellido Paterno:</div>
            <div className="grid-item" style={{ gridColumn: 1, gridRow: 4 }}>Apellido Materno:</div>
            <div className="grid-item" style={{ gridColumn: 1, gridRow: 5 }}>Nivel Educativo:</div>
            <div className="grid-item" style={{ gridColumn: 1, gridRow: 6 }}>Teléfono:</div>
            <div className="grid-item" style={{ gridColumn: 1, gridRow: 7 }}>País:</div>
            <div className="grid-item" style={{ gridColumn: 1, gridRow: 8 }}>Estado:</div>
            <div className="grid-item" style={{ gridColumn: 1, gridRow: 9 }}>Municipio:</div>
            <div className="grid-item" style={{ gridColumn: 1, gridRow: 10 }}>Colonia:</div>
            <div className="grid-item" style={{ gridColumn: 1, gridRow: 11 }}>Calle:</div>
            <div className="grid-item" style={{ gridColumn: 1, gridRow: 12 }}>Acción:</div>

            <div className="grid-item" style={{ gridColumn: 2, gridRow: 1 }}>
                <Input
                    placeholder="CURP"
                    value={curpEditable}
                    onChange={(e) => setCurpEditable(e.target.value)}
                    disabled={!!curpProp} // Deshabilita el input si se ha pasado una CURP como prop
                />
            </div>
            <div className="grid-item" style={{ gridColumn: 2, gridRow: 2 }}>
                <Input placeholder="Nombre" value={nombre} onChange={(e) => setNombre(e.target.value)} />
            </div>
            <div className="grid-item" style={{ gridColumn: 2, gridRow: 3 }}>
                <Input placeholder="Apellido Paterno" value={paterno} onChange={(e) => setPaterno(e.target.value)} />
            </div>
            <div className="grid-item" style={{ gridColumn: 2, gridRow: 4 }}>
                <Input placeholder="Apellido Materno" value={materno} onChange={(e) => setMaterno(e.target.value)} />
            </div>
            <div className="grid-item" style={{ gridColumn: 2, gridRow: 5 }}>
                <Select
                    style={{ width: '100%' }}
                    options={nivelesEducativos}
                    placeholder="Selecciona un nivel educativo"
                    value={nivelEducativoSeleccionado}
                    onChange={setNivelEducativoSeleccionado}
                />
            </div>
            <div className="grid-item" style={{ gridColumn: 2, gridRow: 6 }}>
                <Input placeholder="Télefono" value={telefono} onChange={(e) => setTelefono(e.target.value)} />
            </div>
            <div className="grid-item" style={{ gridColumn: 2, gridRow: 7 }}>
                <Select
                    style={{ width: '100%' }}
                    value={paisSeleccionado}
                    onChange={value => {
                        setPaisSeleccionado(value);
                        setEstadoSeleccionado(null);
                    }}
                    options={paises}
                    placeholder="Selecciona un país"
                />
            </div>
            <div className="grid-item" style={{ gridColumn: 2, gridRow: 8 }}>
                <Select
                    style={{ width: '100%' }}
                    value={estadoSeleccionado}
                    onChange={value => setEstadoSeleccionado(value)}
                    options={estados}
                    placeholder="Selecciona un estado"
                    disabled={!paisSeleccionado}
                />

            </div>
            <div className="grid-item" style={{ gridColumn: 2, gridRow: 9 }}>
                <Select
                    style={{ width: '100%' }}
                    value={municipioSeleccionado}
                    onChange={value => setMunicipioSeleccionado(value)}
                    options={municipios}
                    placeholder="Selecciona un municipio"
                    disabled={!estadoSeleccionado}
                />
            </div>
            <div className="grid-item" style={{ gridColumn: 2, gridRow: 10 }}>
                <Input placeholder="Colonia" value={colonia} onChange={(e) => setColonia(e.target.value)} />
            </div>
            <div className="grid-item" style={{ gridColumn: 2, gridRow: 11 }}>
                <Input placeholder="Calle" value={calle} onChange={(e) => setCalle(e.target.value)} />
            </div>
            <div className="grid-item" style={{ gridColumn: 2, gridRow: 12 }}>
                <Select
                    style={{ width: '100%' }}
                    onChange={setAccionSeleccionada}
                    options={opcionesAcciones}
                    placeholder="Selecciona una Acción"
                />
            </div>
            <div className="grid-item" style={{ gridColumn: 2, gridRow: 13 }}>
                <Button type="primary" onClick={onButtonClick}>
                    {obtenerTextoBoton()}
                </Button>
            </div>
        </div>
    );
}

export default DataHubEstudiante;
