import React, { useState, useEffect } from 'react';
import { Input } from 'antd';
import { Select, Space } from 'antd';
import { Button, Flex } from 'antd';
import { useUser } from '../contexts/UserContext';
import { message } from 'antd';
import "../styles/RegistroTramite.css"
import axios from 'axios';
import { jsPDF } from "jspdf";


const RegistroTramite = ({ curp }) => {

    const [asuntos, setAsuntos] = useState([]);
    const [tramiteSeleccionado, setTramiteSeleccionado] = useState(null);
    const [municipios, setMunicipios] = useState([]);
    const [municipioSeleccionado, setMunicipioSeleccionado] = useState(null);
    const [resultadoTramite, setResultadoTramite] = useState(null);


    const descargarPDF = () => {
        if (resultadoTramite) {
            const doc = new jsPDF();
            doc.text(`Folio del Trámite: ${resultadoTramite.folio}`, 10, 10);
            doc.text(`Fecha del Trámite: ${new Date(resultadoTramite.fechaDeTramite).toLocaleDateString()}`, 10, 20);
            doc.text(`Asunto del Trámite: ${resultadoTramite.descripcionAsunto}`, 10, 30);
            doc.text(`Número de Turno: ${resultadoTramite.numeroTurno}`, 10, 40); 

            doc.save(`Tramite-${resultadoTramite.folio}.pdf`); 
        }
    };



    const agendarTramite = async () => {
        if (!tramiteSeleccionado || !municipioSeleccionado) {
            message.error("Por favor, selecciona un trámite y un municipio.");
            return;
        }

        try {
            const response = await axios.post('http://localhost:44429/weatherforecast/crearTicket2', {
                CURP: curp,
                idasunto: tramiteSeleccionado,
                idmunicipio: municipioSeleccionado,
                Estatus: "En Progreso"
            });

            setResultadoTramite(response.data);
            message.success("Trámite agendado con éxito!");
        } catch (error) {
            console.error('Error al agendar el trámite:', error);
            message.error("Error al agendar el trámite.");
        }
    }

    useEffect(() => {
        const cargarAsuntosYMunicipios = async () => {
            try {
                // Cargar asuntos
                const responseAsuntos = await axios.get('http://localhost:44429/weatherforecast/GetAsuntos');
                setAsuntos(responseAsuntos.data.map(asunto => ({
                    label: asunto.descripcion,
                    value: asunto.idAsunto
                })));

                // Cargar municipios
                if (curp) {
                    const responseMunicipios = await axios.get(`http://localhost:44429/weatherforecast/GetMunicipiosPorCURP/${curp}`);
                    setMunicipios(responseMunicipios.data.map(municipio => ({
                        label: municipio.nombreMunicipio,
                        value: municipio.idMunicipio
                    })));
                }
            } catch (error) {
                console.error('Hubo un error al cargar los datos:', error);
            }
        };

        cargarAsuntosYMunicipios();
    }, [curp]); // Dependencia del useEffect


    const handleSelectChange = value => {
        setTramiteSeleccionado(value);
    }

    return (
        <div className="grid-container3">
            <div className="grid-item3" style={{ gridColumn: 1, gridRow: 1 }}>CURP:</div>
            <div className="grid-item3" style={{ gridColumn: 2, gridRow: 1 }}>
                <Input placeholder="CURP" value={curp} disabled />
            </div>
            <div className='grid-item3' style={{ gridColumn: 1, gridRow: 2 }}>Tramite:</div>
            <div className='grid-item3' style={{ gridColumn: 2, gridRow: 2 }}>

                <Select
                    style={{ width: '100%' }}
                    options={asuntos}
                    placeholder="Selecciona un trámite"
                    value={tramiteSeleccionado}
                    onChange={handleSelectChange}
                />
            </div>
            <div className='grid-item3' style={{ gridColumn: 1, gridRow: 3 }}>Municipio:</div>
            <div className='grid-item3' style={{ gridColumn: 2, gridRow: 3 }}>
                <Select
                    style={{ width: '100%' }}
                    options={municipios}
                    placeholder="Selecciona un Municipio"
                    value={municipioSeleccionado}
                    onChange={setMunicipioSeleccionado}
                />

            </div>
            <div className='grid-item3' style={{ gridColumn: 2, gridRow: 4 }}>
                <Button type="primary" onClick={agendarTramite}>
                    Agendar Tramite
                </Button>
                {resultadoTramite && (
                    <div style={{ gridColumnStart: 1, gridColumnEnd: 3, gridRow: 5, marginTop: '10px' }}>
                        <p>Folio del Trámite: {resultadoTramite.folio}</p>
                        <p>Fecha del Trámite: {new Date(resultadoTramite.fechaDeTramite).toLocaleDateString()}</p>
                        <p>Asunto del Trámite: {resultadoTramite.descripcionAsunto}</p>
                        <p>Número de Turno: {resultadoTramite.numeroTurno}</p> 
                        <Button onClick={descargarPDF}>Descargar PDF</Button>
                    </div>
                )}
            </div>
        </div>
    )
}

export default RegistroTramite
