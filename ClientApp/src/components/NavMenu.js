import React, { useState } from 'react';
import { Collapse, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import { Modal } from 'antd';
import { Input, message } from 'antd';
import axios from 'axios';
import { useUser } from '../contexts/UserContext';

export function NavMenu() {
  const [collapsed, setCollapsed] = useState(true);
  const [modal2Open, setModal2Open] = useState(false);
  const [Username, setUsername] = useState('');
  const [Password, setPassword] = useState('');
  const { setUserType } = useUser();

  const toggleNavbar = () => {
    setCollapsed(!collapsed);
  };

  const handleLogin = async () => {
    try {
      const response = await axios.post(`http://localhost:44429/weatherforecast/login`, {
        Username,
        Password
      });

      setUserType(response.data.userType); // Actualiza el estado userType con la respuesta de la API
      
      if (response.data.userType === 'administrador') {
        message.success('Bienvenido administrador');
      } else if (response.data.userType === 'normal') {
        message.success('Bienvenido usuario');
      } else {
        message.error('Tipo de usuario desconocido');
      }
      
      setModal2Open(false); // Opcional: cierra el modal después del mensaje
    } catch (error) {
      console.error('Error al iniciar sesión:', error);
      message.error('Error al iniciar sesión');
      // Opcional: mantener el modal abierto si hay un errora
    }
  };

  return (
    <header>
      <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
        <NavbarBrand tag={Link} to="/">Tramites</NavbarBrand>
        <NavbarToggler onClick={toggleNavbar} className="mr-2" />
        <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!collapsed} navbar>
          <ul className="navbar-nav flex-grow">
            <NavItem>
              <NavLink tag={Link} className="text-dark" to="/">Home</NavLink>
            </NavItem>
            <NavItem>
              <NavLink className="text-dark cursor-pointer" onClick={() => setModal2Open(true)} >AdminHub</NavLink>
            </NavItem>
            {/* <NavItem>
                <NavLink tag={Link} className="text-dark" to="/fetch-data">Fetch data</NavLink>
              </NavItem> */}
          </ul>
        </Collapse>
      </Navbar>

      <Modal
      title="Login"
      centered
      open={modal2Open}
      onOk={handleLogin}
      onCancel={() => setModal2Open(false)}
    >
      <p className='textd'>Ingresa Usuario</p>
      <Input className='textd' value={Username} onChange={e => setUsername(e.target.value)} />
      <p className='textd'>Ingresa Contraseña</p>
      <Input.Password className='textd' value={Password} onChange={e => setPassword(e.target.value)} />
    </Modal>
    </header>
  );
}
