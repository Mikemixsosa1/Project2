import React from 'react'
import { Collapse } from 'antd';
import DataHubEstudiante from './DataHubEstudiante';
import RegistroTramite from './RegistroTramite.jsx';
import AccionesTramite from './AccionesTramite.jsx';

const TramiteFlow = ({curp}) => {
  const text = `
  A dog is a type of domesticated animal.
  Known for its loyalty and faithfulness,
  it can be found as a welcome guest in many households across the world.
`;
  const items = [
    {
      key: '1',
      label: 'Registro Tramite',
      children: <RegistroTramite curp={curp}/>,
    },
    {
      key: '2',
      label: 'Acciones Tramite',
      children: <AccionesTramite curp={curp}/>,
    },
    {
      key: '3',
      label: 'Acciones Estudiante',
      children: <DataHubEstudiante curpProp={curp}/>,
    }
  ];


  const onChange = (key) => {
    console.log(key);
  };


  return (
    <div>
      <Collapse items={items} defaultActiveKey={['1']} onChange={onChange} />
    </div>
  )
}

export default TramiteFlow
