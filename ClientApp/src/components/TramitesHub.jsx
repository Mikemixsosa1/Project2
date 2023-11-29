import React, { useState } from 'react';
import TramiteFlow from './TramiteFlow'; // Componente para el flujo de trámites
import EstudianteRegistro from './EstudianteRegistro'; // Componente para registrar estudiante
import { Input, Button, Row, Col, Modal } from 'antd';
import DataHubEstudiante from './DataHubEstudiante';
import { message } from 'antd';



const TramitesHub = () => {
    const [curp, setCurp] = useState('');
    const [estudianteExiste, setEstudianteExiste] = useState(undefined);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const colSpan = estudianteExiste ? 12 : 6;

    const showModal = () => {
        setIsModalOpen(true);
    };

    const handleOk = () => {
        setIsModalOpen(false);
    };

    const handleCancel = () => {
        setIsModalOpen(false);
    };

    const validarCurp = (curp) => {
        const curpRegex = /^[A-Z]{4}[0-9]{6}[H,M][A-Z]{5}[A-Z0-9]{2}$/;
        return curpRegex.test(curp);
    };


    const buscarEstudiante = async () => {
        try {
            if (!validarCurp(curp)) {
                message.error('La CURP ingresada no es válida. Por favor, verifica la CURP e inténtalo de nuevo.');
                return;
            }
            const response = await fetch(`http://localhost:44429/weatherforecast/findstudent/${curp}`);
            if (response.ok) {
                const data = await response.json();
                setEstudianteExiste(true);
            } else {
                setEstudianteExiste(false);
            }
        } catch (error) {
            console.error("Hubo un problema con la petición Fetch: " + error.message);
        }
    }

    return (
        <div>
            <Row justify="center" gutter={[8, 16]} className="gutter-row">
                <Col span={8}>
                    <h1 style={{ textAlign: 'center', marginBottom: '16px' }}>Trámites Hub</h1>
                </Col>
            </Row>
            <Row justify="center" gutter={[8, 16]} className="gutter-row">
                <Col span={6}>
                    <Input
                        type="text"
                        placeholder="Ingresa la CURP del estudiante"
                        value={curp}
                        onChange={(e) => setCurp(e.target.value)}
                        style={{ justifySelf: 'center', marginBottom: '8px' }}
                    />
                </Col>
            </Row>
            <Row justify="center" gutter={[8, 16]} className="gutter-row">
                <Col span={colSpan} style={{ textAlign: 'center' }}>
                    <Button type="primary" onClick={buscarEstudiante}>Buscar</Button>
                    {estudianteExiste === undefined && (
                        <p>Por favor, ingresa una CURP para comenzar.</p>
                    )}
                    {estudianteExiste === true && (
                        <div style={{ textAlign: 'center', marginTop: '16px' }}>
                            <TramiteFlow curp={curp} />
                        </div>
                    )}
                    {estudianteExiste === false && (
                        <div style={{ textAlign: 'center', marginTop: '16px' }}>
                            <p>Estudiante no encontrado.</p>
                            <Button type="primary" onClick={showModal}>Registrar Estudiante</Button>
                        </div>
                    )}
                </Col>
            </Row>

            <Modal title="Registro de Estudiante" open={isModalOpen} onOk={handleOk} onCancel={handleCancel}>
                <DataHubEstudiante curpProp={curp} />
            </Modal>


        </div>
    );
}

export default TramitesHub;
