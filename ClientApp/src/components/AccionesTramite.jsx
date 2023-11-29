import React, { useState, useEffect } from 'react';
import "../styles/AccionesTramite.css"
import { Input } from 'antd';
import { Select, Space } from 'antd';
import { Button, Flex } from 'antd';
import { useUser } from '../contexts/UserContext';
import { message } from 'antd';
import { InputNumber } from 'antd';
import { DatePicker } from 'antd';
import moment from 'moment';



const AccionesTramite = ({ curp }) => {
    const { userType } = useUser();
    const [accionSeleccionada, setAccionSeleccionada] = useState(null);
    const [folio, setFolio] = useState(null);
    const [estatus, setEstatus] = useState(null);
    const [fechaTramite, setFechaTramite] = useState(null);
    const [municipio, setMunicipio] = useState(null);
    const [turno, setTurno] = useState(null);
    const [idAsunto, setIdAsunto] = useState(null);
    const [asuntos, setAsuntos] = useState([]);
    const [municipios, setMunicipios] = useState([]);

    const cargarMunicipios = async () => {
        try {
            const response = await fetch(`http://localhost:44429/weatherforecast/GetMunicipiosPorCURP/${curp}`);
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const data = await response.json();
            setMunicipios(data);
        } catch (error) {
            message.error('Hubo un error al cargar los municipios: ' + error.message);
        }
    };



    useEffect(() => {
        const cargarAsuntos = async () => {
            try {
                const response = await fetch('http://localhost:44429/weatherforecast/GetAsuntos');
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                const data = await response.json();
                setAsuntos(data);
            } catch (error) {
                message.error('Hubo un error al cargar los asuntos: ' + error.message);
            }
        };

        cargarAsuntos();
    }, []);

    useEffect(() => {
        cargarMunicipios(); // Cargar municipios al montar el componente y cada vez que cambie el CURP
    }, [curp]); // Asegúrate de incluir 'curp' en el arreglo de dependencias


    const opcionesDeAsuntos = asuntos.map(asunto => ({
        value: asunto.idAsunto,
        label: asunto.descripcion
    }));


    const handleFolioChange = value => setFolio(value);
    const handleChangeEstatus = (value) => {
        setEstatus(value);
    };
    const handleFechaTramiteChange = (date, dateString) => {
        // Si solo necesitas el string de la fecha, esto está bien.
        // setFechaTramite(dateString);

        // Pero si necesitas manipular la fecha después o reutilizarla con el DatePicker,
        // es mejor guardar el objeto `moment`:
        setFechaTramite(date);
    };

    const handleMunicipioChange = value => setMunicipio(value);
    const handleTurnoChange = value => setTurno(value);
    const handleIdAsuntoChange = value => setIdAsunto(value);

    const accionesCRUD = [
        { value: 'leer', label: 'Buscar' },
        { value: 'actualizar', label: 'Actualizar' },
    ];

    const opcionesAcciones = userType === 'administrador' ? accionesCRUD : accionesCRUD.filter(accion => accion.value === 'leer' || accion.value === 'crear');

    const estatusOptions = [
        { value: 'En Proceso', label: 'En Proceso' },
        { value: 'Completado', label: 'Completado' },
        { value: 'Cancelado', label: 'Cancelado' }
    ];

    const estatusOpciones = userType === 'administrador' ? estatusOptions : estatusOptions.filter(estatus => estatus.value == 'Cancelado')


    const dateFormat = 'MM/DD/YYYY';

    const buscarTicket = async () => {
        if (!folio || !curp) {
            message.error('Por favor, ingresa el folio y la CURP para buscar.');
            return;
        }

        try {



            const response = await fetch(`http://localhost:44429/weatherforecast/buscarticket?folio=${folio}&curp=${curp}`);
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const data = await response.json();
            setEstatus(data.estatus);
            setFechaTramite(moment(data.fechaDeTramite));
            setMunicipio(data.idMunicipio);
            setTurno(data.numeroTurno);
            setIdAsunto(data.idAsunto);
        } catch (error) {
            message.error('Hubo un error al buscar el ticket: ' + error.message);
        }
    };






    const ejecutarAccion = async () => {
        if (accionSeleccionada === 'leer') {
            buscarTicket();
        } else if (accionSeleccionada === 'actualizar') {
            if (!folio) {
                message.error('Por favor, ingresa el folio para actualizar.');
                return;
            }

            try {
                const ticketActualizado = {
                    Folio: folio,
                    CURP: curp, // Asumiendo que CURP no cambia
                    IdAsunto: idAsunto,
                    Estatus: estatus,
                    // No enviamos 'NumeroTurno' ni 'FechaRegistroTramite' ya que se manejan en el backend
                    IdMunicipio: municipio,
                    // 'FechaDeTramite' también se establece en el backend
                };

                const response = await fetch('http://localhost:44429/weatherforecast/actualizarTicket', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(ticketActualizado),
                });

                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }


                const data = await response.json();
                message.success('Ticket actualizado con éxito');
                console.log('Respuesta de actualización:', data);

                buscarTicket();



            } catch (error) {
                message.error('Hubo un error al actualizar el ticket: ' + error.message);
            }
        } else {
            // Maneja las otras acciones (crear, eliminar)
        }
    };



    return (
        <div className="grid-container2">
            <div className="grid-item2" style={{ gridColumn: 1, gridRow: 1 }}>
                Folio:
            </div>
            <div className="grid-item2" style={{ gridColumn: 2, gridRow: 1 }}>
                <InputNumber min={1} value={folio} onChange={handleFolioChange} />
            </div>
            <div className="grid-item2" style={{ gridColumn: 1, gridRow: 2 }}>
                CURP:
            </div>
            <div className="grid-item2" style={{ gridColumn: 2, gridRow: 2 }}>
                <Input placeholder="CURP" value={curp} disabled />
            </div>
            <div className="grid-item2" style={{ gridColumn: 1, gridRow: 3 }}>
                Tramite:
            </div>
            <div className="grid-item2" style={{ gridColumn: 2, gridRow: 3 }}>
                <Select
                    onChange={handleIdAsuntoChange}
                    value={idAsunto}
                    style={{ width: '100%' }}
                    options={opcionesDeAsuntos}
                />

            </div>
            <div className="grid-item2" style={{ gridColumn: 1, gridRow: 4 }}>
                Estatus:
            </div>
            <div className="grid-item2" style={{ gridColumn: 2, gridRow: 4 }}>
                <Select
                    defaultValue={estatus}
                    style={{ width: '100%' }}
                    onChange={handleChangeEstatus}
                    options={estatusOpciones}
                    value={estatus}
                />
            </div>
            <div className="grid-item2" style={{ gridColumn: 1, gridRow: 5 }}>
                Fecha de Tramite:
            </div>
            <div className="grid-item2" style={{ gridColumn: 2, gridRow: 5 }}>
                <DatePicker
                    onChange={handleFechaTramiteChange}
                    value={fechaTramite ? moment(fechaTramite, dateFormat) : null}
                    style={{ width: '100%' }}
                    disabled
                    format={dateFormat}
                />



            </div>
            <div className="grid-item2" style={{ gridColumn: 1, gridRow: 6 }}>
                Municipio:
            </div>
            <div className="grid-item2" style={{ gridColumn: 2, gridRow: 6 }}>
                <Select
                    onChange={handleMunicipioChange}
                    value={municipio}
                    style={{ width: '100%' }}
                    options={municipios.map(municipio => ({
                        value: municipio.idMunicipio,
                        label: municipio.nombreMunicipio
                    }))}
                />


            </div>
            <div className="grid-item2" style={{ gridColumn: 1, gridRow: 7 }}>
                Turno:
            </div>
            <div className="grid-item2" style={{ gridColumn: 2, gridRow: 7 }}>
                <InputNumber min={1} style={{ width: '100%' }} value={turno} onChange={handleTurnoChange} disabled />
            </div>
            <div className="grid-item2" style={{ gridColumn: 1, gridRow: 8 }}>
                Acción:
            </div>
            <div className="grid-item2" style={{ gridColumn: 2, gridRow: 8 }}>
                <Select style={{ width: '100%' }}
                    onChange={setAccionSeleccionada}
                    options={opcionesAcciones}
                    placeholder="Selecciona una Acción"
                ></Select>
            </div>
            <div className="grid-item2" style={{ gridColumn: 2, gridRow: 9 }}>
                <Button type="primary" onClick={ejecutarAccion}>
                    Ejecutar Acción
                </Button>

            </div>
        </div>
    )
}

export default AccionesTramite
